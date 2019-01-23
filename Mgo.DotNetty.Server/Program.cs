using Mgo.DotNetty.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            DotNettyServer _netty = new DotNettyServer(10228);
            _netty.OnRecieveServiceRequest += _netty_OnRecieveServiceRequest;
            _netty.StartAsync();

            Console.ReadLine();



        }

        private static DotNettyData _netty_OnRecieveServiceRequest(DotNettyData data)
        {

            DotNettyData res = new DotNettyData();

            Console.WriteLine($"收到客户端消息:[{ data.Data.ToString() }]");

            res.Data = "[" + data.Data.ToString() + "]===>服务端返回的消息";

            return res;


        }
    }
}
