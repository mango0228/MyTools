using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class DotNettyServer
    {
        private int port;
        private ServerBootstrapper bootstrapper = null;
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// 接收服务请求事件
        /// </summary>
        public event RecieveServiceRequestDelegate OnRecieveServiceRequest;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="port"></param>
        public DotNettyServer(int port)
        {
            this.port = port;
            this.bootstrapper = new ServerBootstrapper(port);

        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync() => await bootstrapper.RunAsync(Environment.ProcessorCount, cts.Token,OnRecieveServiceRequest);

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Stop()
        {
            bootstrapper.CloseCompletion.Wait(TimeSpan.FromSeconds(0));
        }
    }
}
