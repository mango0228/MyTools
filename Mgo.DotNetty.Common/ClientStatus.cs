using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    /// <summary>
    /// 客户端状态
    /// </summary>
    public enum ClientStatus
    {
        /// <summary>
        /// 主动关闭
        /// </summary>
        Closed = 1,

        /// <summary>
        /// 连接中
        /// </summary>
        Connecting = 2,

        /// <summary>
        /// 已连接
        /// </summary>
        Connected = 3,

        /// <summary>
        /// 被动关闭
        /// </summary>
        PassiveClosed = 4
    }
}
