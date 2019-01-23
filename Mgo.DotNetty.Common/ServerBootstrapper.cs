using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Concurrency;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    internal class ServerBootstrapper
    {
        const int ListenBacklogSize = 100;

        private int port;
        readonly TaskCompletionSource closeCompletionSource;

        internal ServerBootstrapper(int port)
        {
            this.port = port;
            closeCompletionSource = new TaskCompletionSource();
           
        }

        IEventLoopGroup parentEventLoopGroup;
        IEventLoopGroup eventLoopGroup;
        IChannel serverChannel;


        internal Task CloseCompletion => this.closeCompletionSource.Task;

        public async Task RunAsync(int threadCount, CancellationToken cancellationToken, RecieveServiceRequestDelegate recieveServiceRequest)
        {
            Contract.Requires(threadCount > 0);
            try
            {
                LogsManager.Info("通用服务开始启动!");
                this.parentEventLoopGroup = new MultithreadEventLoopGroup(1);
                this.eventLoopGroup = new MultithreadEventLoopGroup(threadCount);
                ServerBootstrap bootstrap = this.SetupBootstrap(recieveServiceRequest);

                this.serverChannel = await bootstrap.BindAsync(port);

                cancellationToken.Register(this.CloseAsync);
                LogsManager.Info("通用服务成功启动!");
            }
            catch (Exception ex)
            {
                LogsManager.Error("通用服务启动失败");
                this.CloseAsync();
            }

        }

        ServerBootstrap SetupBootstrap(RecieveServiceRequestDelegate recieveServiceRequest)
        {
            var serviceRecieveHandler = CreateRecieveServiceRequestHandler(recieveServiceRequest);

            return new ServerBootstrap()
              .Group(this.parentEventLoopGroup, this.eventLoopGroup)
              .Channel<TcpServerSocketChannel>()
              .Option(ChannelOption.SoBacklog, ListenBacklogSize)
              .ChildOption(ChannelOption.TcpNodelay, true)
              .ChildOption(ChannelOption.SoKeepalive, true)
              .ChildOption(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
              .Handler(new LoggingHandler("CommServerErrror", LogLevel.ERROR))
              .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
              {
                  IChannelPipeline pipeline = channel.Pipeline;
                  //pipeline.AddLast("IdleStateHandler", new IdleStateHandler(0, 0, 30));
                  pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                  pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(Int32.MaxValue, 0, 4, 0, 4));
                  pipeline.AddLast("comm-ecode", new CommEncoder());
                  pipeline.AddLast("comm-decode", new CommDecoder());
                  pipeline.AddLast("comm-handler", new ServerServiceHandler(serviceRecieveHandler));
                  //pipeline.AddLast("IdleStateHearBeatReqHandler", new IdleStateHearBeatReqHandler());

              }));
        }


        private Func<DotNettyData, DotNettyData> CreateRecieveServiceRequestHandler(RecieveServiceRequestDelegate recieveServiceRequestDelegate)
        {
            if (recieveServiceRequestDelegate == null)
            {
                return null;
            }

            return new Func<DotNettyData, DotNettyData>((message) =>
            {
                return recieveServiceRequestDelegate(message);
            });
        }

        async void CloseAsync()
        {
            try
            {
                if (this.serverChannel != null)
                {
                    await this.serverChannel.CloseAsync();
                }
                if (this.eventLoopGroup != null)
                {
                    await this.eventLoopGroup.ShutdownGracefullyAsync();
                }

            }
            catch (Exception ex)
            {
                LogsManager.Error("通用服务停止失败");
            }
            finally
            {
                this.closeCompletionSource.TryComplete();
            }
        }
    }
}
