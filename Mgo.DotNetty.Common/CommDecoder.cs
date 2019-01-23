using DotNetty.Buffers;
using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace Mgo.DotNetty.Common
{
    internal class CommDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer directBuf, List<object> output)
        {
            int length = directBuf.ReadableBytes;
            if (length <= 0)
            {
                return;
            }
            try
            {
                if (directBuf.HasArray)
                {
                    byte[] bytes = new byte[length];
                    directBuf.GetBytes(directBuf.ReaderIndex, bytes);
                    output.Add(bytes);
                }
            }
            catch (Exception ex)
            {
                LogsManager.Error(ex,"");
            }
        }
    }
}
