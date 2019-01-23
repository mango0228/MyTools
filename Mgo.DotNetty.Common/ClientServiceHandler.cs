using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    internal class ClientServiceHandler : SimpleChannelInboundHandler<byte[]>
    {
        private RequestManager requestManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="requestManager">请求管理器</param>
        internal ClientServiceHandler(RequestManager requestManager)
        {
            this.requestManager = requestManager;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, byte[] msg)
        {
            ThreadPool.QueueUserWorkItem((object state) =>
            {
                try
                {
                    byte[] bytes = (byte[])state;
                    DotNettyData result = (DotNettyData)ByteExtension.ByteArrayToObject(bytes);
                    requestManager.CompleteRequest(result.SerialNumber, result);
                }
                catch (Exception ex)
                {
                    LogsManager.Error("处理服务端结果失败");
                }

            }, msg);
        }
    }
}
