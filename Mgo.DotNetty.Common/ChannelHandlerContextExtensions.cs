using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    /// <summary>
    /// ChannelHandler扩展方法
    /// </summary>
    public static class ChannelHandlerContextExtensions
    {
        /// <summary>
        /// 获取远程地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IPEndPoint GetRemoteAddress(this IChannelHandlerContext context)
        {
            return (IPEndPoint)context.Channel.RemoteAddress;
        }

        /// <summary>
        /// 获取远程IP与端口（IP:Port）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRemoteNetString(this IChannelHandlerContext context)
        {
            var address = context.GetRemoteAddress();
            return $"{address.Address.ToString()}:{address.Port}";
        }

        /// <summary>
        /// 获取远程IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRemoteIP(this IChannelHandlerContext context)
        {
            var address = context.GetRemoteAddress();
            return address.Address.ToString();
        }

        /// <summary>
        /// 获取远程端口
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetRemotePort(this IChannelHandlerContext context)
        {
            var address = context.GetRemoteAddress();
            return address.Port;
        }

        /// <summary>
        /// 获取本地地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IPEndPoint GetLocalAddress(this IChannelHandlerContext context)
        {
            return (IPEndPoint)context.Channel.LocalAddress;
        }

        /// <summary>
        /// 获取本地IP和端口（IP:Port）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetLocalNetString(this IChannelHandlerContext context)
        {
            var address = context.GetLocalAddress();
            return $"{address.Address.ToString()}:{address.Port}";
        }

        /// <summary>
        /// 获取本地IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetLocalIP(this IChannelHandlerContext context)
        {
            var address = context.GetLocalAddress();
            return address.Address.ToString();
        }

        /// <summary>
        /// 获取本地端口
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetLocalPort(this IChannelHandlerContext context)
        {
            var address = context.GetLocalAddress();
            return address.Port;
        }

        /// <summary>
        /// 获取Channel名称
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetChannelName(this IChannelHandlerContext context)
        {
            return context.GetRemoteNetString();
        }
    }

}