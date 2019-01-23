using Mgo.DotNetty.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Client
{
    class Program
    {
        static void Main(string[] args)
        {

            DotNettyClient client = new DotNettyClient("127.0.0.1",10228);
            client.StartAsync();

            Thread.Sleep(2000);

            DotNettyData paramData = new DotNettyData { Data = "i am client message", SerialNumber = Guid.NewGuid().ToString() };

            DotNettyData result = client.SendAsync(paramData, false, 10000).Result; //设定30秒超时

            Console.WriteLine(result.Data.ToString());

            Console.ReadLine();

            client.CloseAsync();



        }
    }
}
