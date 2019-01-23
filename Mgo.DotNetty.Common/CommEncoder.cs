using DotNetty.Codecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Mgo.DotNetty.Common
{
    internal class CommEncoder : MessageToByteEncoder<DotNettyData>
    {
        protected override void Encode(IChannelHandlerContext context, DotNettyData message, IByteBuffer output)
        {
            byte[] bytes = ByteExtension.ObjectToByteArray(message);
            output.WriteBytes(bytes);
        }

    }
}
