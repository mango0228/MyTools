using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    /// <summary>
    /// 客户端异常Handler
    /// </summary>
    public class ClientExceptionHandler : ChannelHandlerAdapter
    {

        private Func<Task> socketExceptionHandler;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socketExceptionHandler">网络异常Handler</param>
        public ClientExceptionHandler(Func<Task> socketExceptionHandler)
        {
            this.socketExceptionHandler = socketExceptionHandler;
        }

        public async override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            if (exception is SocketException)
            {
                LogsManager.Error(exception, $"Socket exception. Local={context.GetLocalNetString()}, Remote={context.GetRemoteNetString()}, ExceptionMessage={exception.Message}");
                await socketExceptionHandler?.Invoke();
            }
            else
            {
                LogsManager.Error(exception, $"Unhandle exception. Local={context.GetLocalNetString()}, Remote={context.GetRemoteNetString()}, ExceptionMessage={exception.Message}");
                await context.CloseAsync();
                LogsManager.Info($"Channel closed because has an unhandle exception. Local={context.GetLocalNetString()}, Remote={context.GetRemoteNetString()}");
                context.FireExceptionCaught(exception);
            }
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            LogsManager.Info($"Channel inactived. Local={context.GetLocalNetString()}, Remote={context.GetRemoteNetString()}");
            base.ChannelInactive(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            LogsManager.Info($"Channel unregistered. Local={context.GetLocalNetString()}, Remote={context.GetRemoteNetString()}");
            base.ChannelUnregistered(context);
        }
    }
}