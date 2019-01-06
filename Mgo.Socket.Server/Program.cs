using Mgo.SocketCommon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace Mgo.Socket.Server
{
    class Program
    {
        static void Main(string[] args)
        {

            //创建服务器对象，默认监听本机0.0.0.0，14524
            SocketServer server = new SocketServer(14521);
            //服务器启动
            server.StartServer();
            server.ResponseContent = new Action<string, string, SocketConnection>((key, content, conn) =>
            {
                Console.WriteLine($"收到客户端数据：key={key},content={content}");
                conn.Send(key + content);
            });
            while (true)
            {
                Console.WriteLine("输入:quit，关闭服务器");
                string op = Console.ReadLine();
                if (op == "quit")
                    break;
            }
        }
    }
}
