using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    public class LogsManager
    {


        public static void Debug(string msg)
        {
            Console.WriteLine($"debug:[{msg}]");
        }


        public static void Error(Exception ex, string msg)
        {
            Console.WriteLine($"Error:[{msg}],ex:[{ex.Message}]");
        }

        public static void Error(string msg)
        {
            Console.WriteLine($"Error:[{msg}]");
        }

        public static void Info(Exception ex, string msg)
        {
            Console.WriteLine($"Info:[{msg}],ex:[{ex.Message}]");
        }

        public static void Info(string msg)
        {
            Console.WriteLine($"Info:[{msg}]");
        }





    }
}
