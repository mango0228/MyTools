using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mgo.SocketCommon
{
    /// <summary>
    /// Socket客户端
    /// </summary>
    public class SocketClient
    {



        /// <summary>
        /// 缓冲接收的包
        /// </summary>
        public ConcurrentQueue<byte> BufferByte = new ConcurrentQueue<byte>();


        public ConcurrentDictionary<string, string> ResponseContent = new ConcurrentDictionary<string, string>();

        //public Dictionary<string, string> ResponseContent = new Dictionary<string, string>();

        #region 构造函数

        /// <summary>
        /// 构造函数,连接服务器IP地址默认为本机127.0.0.1
        /// </summary>
        /// <param name="port">监听的端口</param>
        public SocketClient(int port)
        {
            _ip = "127.0.0.1";
            _port = port;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">监听的IP地址</param>
        /// <param name="port">监听的端口</param>
        public SocketClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }


        private string GetIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostByName(hostName);    //方法已过期，可以获取IPv4的地址
            IPAddress localaddr = localhost.AddressList[0];
            return localaddr.ToString();
        }




        #endregion

        #region 内部成员

        private Socket _socket = null;
        private string _ip = "";
        private int _port = 0;
        private bool _isRec=true;
        private bool IsSocketConnected()
        {
            bool part1 = _socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (_socket.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }



        private void InitClient()
        {
            //绑定当收到服务器发送的消息后的处理事件
            this.HandleRecMsg = new Action<byte[], SocketClient>((bytes, theClient) =>
            {

                if (bytes != null && bytes.Length > 0)
                {

                    //缓冲区有剩余包
                    List<byte> bufferList = new List<byte>();
                    while (!theClient.BufferByte.IsEmpty)
                    {
                        byte b = 0;
                        if (theClient.BufferByte.TryDequeue(out b))
                        {
                            bufferList.Add(b);
                        }
                    }
                    //包的字节数据
                    List<byte> bytesContent = new List<byte>();

                    //把剩余的包字节跟新的包合并起来
                    if (bufferList.Count > 0)
                    {
                        //把缓冲区的字节拿出来放到新收到字节的前面
                        bytesContent = bufferList.Concat(bytes).ToList();
                    }
                    else
                    {
                        bytesContent = bytes.ToList();
                    }

                    //此类可以共用一下.
                    IpByteData ibda = new IpByteData();
                    ibda.SetBytes(bytesContent.ToArray());
                    //多余的包字节缓冲起来
                    if (ibda.RemainingBytes.Count > 0)
                    {
                        for (int i = 0; i < ibda.RemainingBytes.Count; i++)
                        {
                            theClient.BufferByte.Enqueue(ibda.RemainingBytes[i]);
                        }
                    }
                    //解压完整的包
                    if (ibda.BytesComplete.Count > 0)
                    {
                        foreach (List<byte> item in ibda.BytesComplete)
                        {
                            string requestKey = "";
                            string responseContent = "";
                            IpByteData.GetContentAndRequetKey(item.ToArray(), out requestKey, out responseContent);
                            //万一没加入成功，则开奖失败， 
                            theClient.ResponseContent.TryAdd(requestKey, responseContent);
                           // theClient.ResponseContent.Add(requestKey, responseContent);

                        }
                    }
                }
            });

            //绑定向服务器发送消息后的处理事件
            this.HandleSendMsg = new Action<byte[], SocketClient>((bytes, theClient) =>
            {
                // Console.WriteLine($"向服务器发送消息:{msg}");
            });

        }



        /// <summary>
        /// 开始接受客户端消息
        /// </summary>
        public void StartRecMsg()
        {
            try
            {
                byte[] container = new byte[1024 * 1024 * 2 ]; //1mb
                _socket.BeginReceive(container, 0, container.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        int length = _socket.EndReceive(asyncResult);

                        //马上进行下一轮接受，增加吞吐量
                        if (length > 0 && _isRec && IsSocketConnected())
                            StartRecMsg();

                        if (length > 0)
                        {
                            byte[] recBytes = new byte[length];
                            Array.Copy(container, 0, recBytes, 0, length);

                            //处理消息
                            HandleRecMsg?.BeginInvoke(recBytes, this,null,null);
                        }
                        else
                            Close();
                    }
                    catch (Exception ex)
                    {
                        HandleException?.BeginInvoke(ex,null,null);
                        Close();
                    }
                }, null);
            }
            catch (Exception ex)
            {
                HandleException?.BeginInvoke(ex,null,null);
                Close();
            }
        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 开始服务，连接服务端
        /// </summary>
        public void StartClient()
        {
            try
            {
                InitClient();

                //实例化 套接字 （ip4寻址协议，流式传输，TCP协议）
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //创建 ip对象
                IPAddress address = IPAddress.Parse(_ip);
                //创建网络节点对象 包含 ip和port
                IPEndPoint endpoint = new IPEndPoint(address, _port);
                //将 监听套接字  绑定到 对应的IP和端口
                _socket.BeginConnect(endpoint, asyncResult =>
                {
                    try
                    {
                        _socket.EndConnect(asyncResult);
                        //开始接受服务器消息
                        StartRecMsg();

                        HandleClientStarted?.BeginInvoke(this,null,null);
                    }
                    catch (Exception ex)
                    {
                        HandleException?.BeginInvoke(ex,null,null);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                HandleException?.BeginInvoke(ex,null,null);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes">数据字节</param>
        public void Send(byte[] bytes)
        {
            try
            {
                _socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        int length = _socket.EndSend(asyncResult);
                        HandleSendMsg?.BeginInvoke(bytes, this,null,null);
                    }
                    catch (Exception ex)
                    {
                        HandleException?.BeginInvoke(ex,null,null);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                HandleException?.BeginInvoke(ex,null,null);
            }
        }


        /// <summary>
        /// 发送字符串（默认使用UTF-8编码）
        /// </summary>
        /// <param name="msgStr">发送的字符串</param>
        /// <param name="guidToN">请求唯一编码【用于后续获取响应的消息，请使用固定32字节的字符串建议使用Guid.ToString("N")的方式赋值】</param>
        public void Send(string msgStr,string guidToN)
        {
            msgStr = guidToN + msgStr;
            //加入包长.
            byte[] bytesend = Encoding.UTF8.GetBytes(msgStr);
            byte[] lengByte = BitConverter.GetBytes(bytesend.Length);
            
            ////把包长度加入发送包中.
            byte[] send = new byte[bytesend.Length + lengByte.Length];
            Array.Copy(lengByte, 0, send, 0, lengByte.Length);
            Array.Copy(bytesend, 0, send, lengByte.Length, bytesend.Length);

            Send(send);
        }


        /// <summary>
        /// 发送字符串（默认使用UTF-8编码）
        /// </summary>
        /// <param name="msgStr">发送的字符串</param>
        /// <param name="guidToN">请求唯一编码【用于后续获取响应的消息，请使用固定32字节的字符串建议使用Guid.ToString("N")的方式赋值】</param>
        public string SendToSyncGetResponseContent(string msgStr)
        {
            string requestKey = Guid.NewGuid().ToString("N");
            //加入包长.
            byte[] bytesend = Encoding.UTF8.GetBytes(requestKey + msgStr);
            byte[] lengByte = BitConverter.GetBytes(bytesend.Length);

            ////把包长度加入发送包中.
            byte[] send = new byte[bytesend.Length + lengByte.Length];
            Array.Copy(lengByte, 0, send, 0, lengByte.Length);
            Array.Copy(bytesend, 0, send, lengByte.Length, bytesend.Length);
            Send(send);

            return SyncGetResponseContent(requestKey);
        }






        /// <summary>
        /// 发送字符串（使用自定义编码）
        /// </summary>
        /// <param name="msgStr">字符串消息</param>
        /// <param name="encoding">使用的编码</param>
        public void Send(string msgStr, Encoding encoding)
        {
            Send(encoding.GetBytes(msgStr));
        }



        public string SyncGetResponseContent(string requestKey, int timeOut = 3000)
        {
            string content = "";
            int timer = 0;
            this.ResponseContent.TryRemove(requestKey, out content);

            while (string.IsNullOrEmpty(content))
            {
                System.Threading.Thread.Sleep(1);
                this.ResponseContent.TryRemove(requestKey, out content);
                if (timer > timeOut)
                {
                    break;
                }
                timer += 1;
            }

            //while (!this.ResponseContent.ContainsKey(requestKey))
            //{
            //    if (this.ResponseContent.ContainsKey(requestKey))
            //    {
            //        content = this.ResponseContent[requestKey];
            //        this.ResponseContent.Remove(requestKey);
            //        break;
            //    }
            //    if (timer > timeOut)
            //    {
            //        break;
            //    }
            //    System.Threading.Thread.Sleep(5);
            //    timer += 5;
            //}
            return content;
        }



        /// <summary>
        /// 传入自定义属性
        /// </summary>
        public object Property { get; set; }

        /// <summary>
        /// 关闭与服务器的连接
        /// </summary>
        public void Close()
        {
            try
            {
                _isRec = false;
                _socket.Disconnect(false);
                HandleClientClose?.BeginInvoke(this,null,null);
            }
            catch (Exception ex)
            {
                HandleException?.BeginInvoke(ex,null,null);
            }
            finally
            {
                _socket.Dispose();
                GC.Collect();
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 客户端连接建立后回调
        /// </summary>
        public Action<SocketClient> HandleClientStarted { get; set; }

        /// <summary>
        /// 处理接受消息的委托
        /// </summary>
        public Action<byte[], SocketClient> HandleRecMsg { get; set; }

        /// <summary>
        /// 客户端连接发送消息后回调
        /// </summary>
        public Action<byte[], SocketClient> HandleSendMsg { get; set; }

        /// <summary>
        /// 客户端连接关闭后回调
        /// </summary>
        public Action<SocketClient> HandleClientClose { get; set; }

        /// <summary>
        /// 异常处理程序
        /// </summary>
        public Action<Exception> HandleException { get; set; }

        #endregion
    }
}
