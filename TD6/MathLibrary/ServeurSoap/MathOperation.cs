using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MathLibrary
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class MathOperation : IMathsOperations
    {
        int IMathsOperations.add(int a, int b)
        {
            return (a + b);
        }

        int IMathsOperations.multiply(int a, int b)
        {
            return (a * b);
        }

        int IMathsOperations.substract(int a, int b)
        {
            return (a - b);
        }
        double IMathsOperations.divide(double a, double b)
        {
            return (a / b);
        }
    }
}
