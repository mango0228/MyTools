using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    internal class ServerServiceHandler : SimpleChannelInboundHandler<byte[]>
    {
        private Func<DotNettyData, DotNettyData> serviceRecieveHandler;

        public ServerServiceHandler(Func<DotNettyData, DotNettyData> serviceRecieveHandler)
        {
            this.serviceRecieveHandler = serviceRecieveHandler;
        }

        protected override void ChannelRead0(IChannelHandlerContext context, byte[] bytes)
        {
            DotNettyData data =(DotNettyData)ByteExtension.ByteArrayToObject(bytes);

            if (serviceRecieveHandler != null && data!=null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        var handlerResult = serviceRecieveHandler(data);
                        if (handlerResult != null)
                        {
                            context.WriteAndFlushAsync(handlerResult);
                        }
                    }
                    catch(Exception ex)
                    {
                        //ex.Error();
                    }
                   
                });
            }
        }
    }
}
