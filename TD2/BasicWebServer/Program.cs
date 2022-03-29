using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Specialized;

namespace BasicServerHTTPlistener
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }


            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate
            {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                foreach (string str in request.Url.Segments)
                {
                    Console.WriteLine(str);
                }

                //get params un url. After ? and between &

                Console.WriteLine(request.Url.Query);

                NameValueCollection requests = HttpUtility.ParseQueryString(request.Url.Query);
                //parse params in url
                foreach (String s in HttpUtility.ParseQueryString(request.Url.Query))
                {
                    Console.WriteLine("{0} = {1}", s, requests[s]);
                }
                /*Console.WriteLine("param1 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param1"));
                Console.WriteLine("param2 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param2"));
                Console.WriteLine("param3 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param3"));
                Console.WriteLine("param4 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param4"));*/

                //
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;


                //implementing the reflexion
                /* part 1 td2*/
                Type type = typeof(MyMethods);
                Console.WriteLine(request.Url.Segments.Length);
                MethodInfo method = type.GetMethod(request.Url.Segments[2]);
                
                    
                /*part 2 td2*/
                 MyMethods c = new MyMethods();

                // Construct a response.
                string[] tabParameters = new string[requests.Count];
                
                for(int i = 0, j =1; i< requests.Count; i++,j++)
                {
                    tabParameters[i] = HttpUtility.ParseQueryString(request.Url.Query).Get("param"+j);
                    Console.WriteLine(tabParameters[i]);
                }
    
                /*tabParameters[0] = HttpUtility.ParseQueryString(request.Url.Query).Get("param1");
                tabParameters[1] = HttpUtility.ParseQueryString(request.Url.Query).Get("param2");
                //tabParameters[2] = HttpUtility.ParseQueryString(request.Url.Query).Get("param3");
                //tabParameters[3] = HttpUtility.ParseQueryString(request.Url.Query).Get("param4");*/

                /* part 2 td2
                 * string responseString = c.getMethodExe(tabParameters);*/
          
                string responseString = (string)method.Invoke(c, tabParameters);//"<HTML><BODY> Hello world!</BODY></HTML>";
        
                //string responseString = c.incr(tabParameters) + "";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();

            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
        }

        public class MyMethods
        {
            public string Method1(string param1, string param2)
            {
                Console.WriteLine("Calling method1");
                return "<html><body> Hello " + param1 + " et " + param2 + " </body></html>";
            }

            public string Method2(string param1, string param2)
            {
                Console.WriteLine("Calling method2");
                return "<html><body> Hello " + param1 + " et " + param2 + " </body></html>";
            }

            public string Method3(string param1, string param2)
            {
                Console.WriteLine("Calling method3");
                return "<html><body> Hello " + param1 + " et " + param2 + " </body></html>";
            }
            //part 3
            public string incr(string param1)
            {
               int val = Int32.Parse(param1);
                return (val + 1).ToString();
            }

            public string getMethodExe(String[] paramsTab)
            {

                //
                // Set up the process with the ProcessStartInfo class.
                // https://www.dotnetperls.com/process
                //
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = @"C:\Users\Utilisateur\source\repos\getMethodExe\bin\Debug\net6.0\getMethodExe.exe"; // Specify exe name.
                start.Arguments = "";
                for (int i = 0; i < paramsTab.Length; i++)
                {
                    start.Arguments = start.Arguments + paramsTab[i] + " "; // Specify arguments.
                }
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                //
                // Start the process.
                //
                string result = "";
                using (Process process = Process.Start(start))
                {
                    //
                    // Read in all the text from the process with the StreamReader.
                    //
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd(); ;

                        return result;
                    }

                }

            }
           
        }
    }
}

