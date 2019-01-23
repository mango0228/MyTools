using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class DotNettyClient
    {
        private Bootstrap bootstrap;

        private IEventLoopGroup group;

        private IChannel channel;

        private bool allowReconnect = false;

        private int isReconnecting = 0;


        private int reconnectCount;

        private int reconnectInterval;

        private ClientStatus status;

        private string host, localHost;

        private int port;

        private int? localPort;

        private RequestManager requestManager;

        /// <summary>
        /// 客户端状态
        /// </summary>
        public ClientStatus Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host">服务端地址</param>
        /// <param name="port">服务端端口</param>
        /// <param name="localHost">本地地址</param>
        /// <param name="localPort">本地端口</param>
        /// <param name="reconnectCount">当发生网络错误时尝试重新连接的次数，-1表示无限，默认为-1</param>
        /// <param name="reconnectInterval">每次尝试重新连接的时间间隔，单位：毫秒，默认为30000毫秒</param>
        public DotNettyClient(string host, int port, string localHost = null, int? localPort = null, int reconnectCount = -1, int reconnectInterval = 30000)
        {
            this.port = port;
            this.host = host;
            this.localPort = localPort;
            this.localHost = localHost;
            this.reconnectCount = reconnectCount;
            this.reconnectInterval = reconnectInterval;
            requestManager = new RequestManager();
        }

        /// <summary>
        /// 向服务端发起连接
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            if (bootstrap == null)
            {
                bootstrap = new Bootstrap();
                group = new MultithreadEventLoopGroup();

                bootstrap.Group(group)
                    .Channel<TcpSocketChannel>()
                     .Option(ChannelOption.TcpNodelay, true)
                 .Option(ChannelOption.SoKeepalive, true)
                .Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new LoggingHandler("ClientManage", LogLevel.ERROR));
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(Int32.MaxValue, 0, 4, 0, 4));
                        pipeline.AddLast("MessageDecoder", new CommDecoder());
                        pipeline.AddLast("MessageEncoder", new CommEncoder());
                        pipeline.AddLast("ServiceResultHandler", new ClientServiceHandler(requestManager));
                        pipeline.AddLast("ExceptionHandler", new ClientExceptionHandler(this.ReconnectAsync));
                    }));
            }

            await DoConnectAsync();

            allowReconnect = true;
        }

        /// <summary>
        /// 执行与服务端连接
        /// </summary>
        /// <returns></returns>
        private async Task DoConnectAsync()
        {
            LogsManager.Debug($"Client connect beginning. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}");

           
            try
            {
                status = ClientStatus.Connecting;
                //发起异步连接操作
                if (!string.IsNullOrEmpty(localHost) && localPort != null)
                {
                    channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(host), port), new IPEndPoint(IPAddress.Parse(localHost), localPort.Value));
                }
                else
                {
                    channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(host), port));
                }

                status = ClientStatus.Connected;
                LogsManager.Debug($"Client connect finished. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}");
            }
            catch (AggregateException ex)
            {
                status = ClientStatus.Closed;
                LogsManager.Error(ex, $"Client connect has error. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}, ExceptionMessage={ex.InnerException.Message}, ExceptionStackTrace={ex.InnerException.StackTrace}");
                throw new NetworkException(host, port, ex.InnerException.Message);
            }
        }

        /// <summary>
        /// 关闭与服务端的连接
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            try
            {
                status = ClientStatus.Closed;
                allowReconnect = false;
                await channel.CloseAsync();
                LogsManager.Debug($"Client closed. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}");
            }
            finally
            {
                await group.ShutdownGracefullyAsync();
                LogsManager.Debug($"Group shutdowned. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}");
            }
        }


        private Task ReconnectAsync()
        {
            return Task.Run(async () =>
            {
                if (reconnectCount == 0 || Interlocked.CompareExchange(ref isReconnecting, 1, 0) == 1)
                {
                    return;
                }

                status = ClientStatus.Closed;
                await channel.CloseAsync();

                LogsManager.Debug($"Client reconnect: close connect. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}");

                int i = 0;

                while (reconnectCount == -1 || i < reconnectCount)
                {
                    if (reconnectCount != -1) { i++; }

                    Thread.Sleep(reconnectInterval);

                    if (!allowReconnect) { break; }

                    try
                    {
                        LogsManager.Debug($"Client reconnect: connecting. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}");
                        await DoConnectAsync();
                        LogsManager.Debug($"Client reconnect: connect success. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        LogsManager.Error(ex, $"Client reconnect: connect error. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}, ExceptionMessage={ex.Message}, ExceptionStackTrace={ex.StackTrace}");
                    }
                }
                isReconnecting = 0;
                LogsManager.Debug($"Client reconnect finished. Host={host}, Port={port}, LocalHost={localHost}, LocalPort={localPort}");
            });
        }


        /// <summary>
        /// 单向发送消息到服务器
        /// </summary>
        /// <param name="msg">消息数据</param>
        /// <param name="timeout">超时时长（毫秒）</param>
        /// <returns></returns>
        public async Task SendOneWayAsync(DotNettyData msg, int timeout = 30000)
        {
            var request = requestManager.CreateRequest(timeout,msg.SerialNumber);
            await channel.WriteAndFlushAsync(msg);
            await request.Task;
        }

        /// <summary>
        /// 发送消息到服务器
        /// </summary>
        /// <param name="msg">消息数据</param>
        /// <param name="continueOnCapturedContext">配置用于等待此 System.Threading.Tasks.Task`1的 awaiter。</param>
        /// <param name="timeout">超时时长（毫秒）</param>
        /// <returns></returns>
        public async Task<DotNettyData> SendAsync(DotNettyData msg, bool continueOnCapturedContext = true, int timeout = 30000)
        {
            var request = requestManager.CreateRequest(timeout, msg.SerialNumber);
            try
            {
                await channel.WriteAndFlushAsync(msg).ConfigureAwait(continueOnCapturedContext);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is ClosedChannelException || ex.InnerException is SocketException)
                {
                    throw new NetworkException(host, port, ex.InnerException.Message);
                }
                throw ex;
            }
            var result = await request.Task;
            return result;
        }

    }
}
