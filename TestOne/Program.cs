using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestOne
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("main start..");
            AsyncMethod();
            Thread.Sleep(1000);
            Console.WriteLine("main end..");

            Console.ReadLine();
        }

        static async void AsyncMethod()
        {
            Console.WriteLine("start async");
            var result = await MyMethod();
            Console.WriteLine("end async");
            //return 1;
        }

        static async Task<int> MyMethod()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Async start:" + i.ToString() + "..");
                await Task.Delay(1000); //模拟耗时操作
            }
            return 0;
        }
    }
}
