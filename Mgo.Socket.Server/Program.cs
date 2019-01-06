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

            //for (int i = 0; i < 100; i++)
            //{
            //    string s = Guid.NewGuid().ToString("N");
            //    byte[] by = Encoding.UTF8.GetBytes(s);
            //    Console.Write(by.Length + " : ");
            //    foreach (var item in by)
            //    {
            //        Console.Write(item);
            //    }
            //    Console.WriteLine();
            //    Thread.Sleep(10);
            //}

            //Console.ReadLine();





            //创建服务器对象，默认监听本机0.0.0.0，14524
            SocketServer server = new SocketServer(14521);

            //处理从客户端收到的消息
            server.HandleRecMsg = new Action<byte[], SocketConnection, SocketServer>((bytes, conn, theServer) =>
            {
                string keyip = conn.ClientIp;
                if (bytes != null && bytes.Length > 0)
                {
                    //此类可以共用一下.
                    IpByteData ibda = new IpByteData();

                    if (!server.BufferByte.ContainsKey(keyip))
                    {
                        ibda.SetBytes(bytes);
                        //多余的包字节缓冲起来
                        if (ibda.RemainingBytes.Count > 0)
                        {
                            server.BufferByte.TryAdd(keyip, ibda.RemainingBytes);
                        }
                        //解压完整的包
                        if (ibda.BytesComplete.Count > 0)
                        {
                            foreach (List<byte> item in ibda.BytesComplete)
                            {
                                string msgAll = Encoding.UTF8.GetString(item.ToArray());
                                //Console.WriteLine($"收到来自【{ conn.ClientIp }】的消息:{msgAll }");
                                //conn.Send(msgAll);

                                string requestKey = "";
                                string responseContent = "";
                                IpByteData.GetContentAndRequetKey(item.ToArray(), out requestKey, out responseContent);
                                server.ResponseContent.Invoke(requestKey, responseContent, conn);

                            }
                        }
                    }
                    else
                    {
                        //如果存在缓冲。 则需要先把缓冲里面的字节拿出来。
                        List<byte> listBufferByte = new List<byte>();
                        if (server.BufferByte.TryRemove(keyip, out listBufferByte))
                        {
                            //把缓冲区的字节拿出来放到新收到字节的前面
                            byte[] bufferBytes = listBufferByte.ToArray();
                            byte[] allBytes = bufferBytes.Concat(bytes).ToArray();
                            ibda.SetBytes(allBytes);

                            //多余的包字节缓冲起来
                            if (ibda.RemainingBytes.Count > 0)
                            {
                                server.BufferByte.TryAdd(keyip, ibda.RemainingBytes);
                            }
                            //解压完整的包
                            if (ibda.BytesComplete.Count > 0)
                            {
                                foreach (List<byte> item in ibda.BytesComplete)
                                {
                                    string msgAll = Encoding.UTF8.GetString(item.ToArray());
                                    Console.WriteLine($"收到来自【{ conn.ClientIp }】的消息【缓冲区】:{msgAll}");
                                }
                            }
                        }
                        else
                        {
                            if (server.BufferByte.ContainsKey(keyip))
                            {
                                //把byte 继续缓冲起来。
                                server.BufferByte[keyip].AddRange(bytes);
                            }
                        }

                    }




                }




                 //conn.Send("服务端收到消息,完毕!");
                // Console.WriteLine($"收到来自【{ conn.ClientIp }】的消息:{msgAll},发包长度是：" + leng);


            });

            //处理服务器启动后事件
            server.HandleServerStarted = new Action<SocketServer>(theServer =>
            {
                Console.WriteLine("服务已启动************");
            });

            //处理新的客户端连接后的事件
            server.HandleNewClientConnected = new Action<SocketServer, SocketConnection>((theServer, theCon) =>
            {
                Console.WriteLine($@"一个新的客户端接入，当前连接数：{theServer.GetConnectionCount()}");
            });

            //处理客户端连接关闭后的事件
            server.HandleClientClose = new Action<SocketConnection, SocketServer>((theCon, theServer) =>
            {
                Console.WriteLine($@"一个客户端关闭，当前连接数为：{theServer.GetConnectionCount()}");
            });

            //处理异常
            server.HandleException = new Action<Exception>(ex =>
            {
                Console.WriteLine(ex.Message);
            });


            server.ResponseContent = new Action<string, string, SocketConnection>((key,content,conn) =>
            {
                Console.WriteLine($"收到客户端数据：key={key},content={content}");
                conn.Send(key + content);
                

            });



            //服务器启动
            server.StartServer();





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
