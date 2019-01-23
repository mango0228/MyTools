using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mgo.DotNetty.Common
{
    /// <summary>
    /// 请求管理器
    /// </summary>
    internal class RequestManager
    {
        private ConcurrentDictionary<string, RequestInfo> requestList = new ConcurrentDictionary<string, RequestInfo>();

        /// <summary>
        /// 创建请求信息对象
        /// </summary>
        /// <param name="timeout">请求超时时长（毫秒）</param>
        /// <param name="serialNumber">唯一序列号</param>
        /// <returns></returns>
        public RequestInfo CreateRequest(int timeout,string serialNumber)
        {
            var request = new RequestInfo();

            requestList.TryAdd(serialNumber, request);

            var tcs = new TaskCompletionSource<DotNettyData>();
            var cts = new CancellationTokenSource(timeout);

            var token = cts.Token;
            token.Register(() =>
            {
                if (!requestList.TryRemove(serialNumber, out request))
                {
                    return;
                }

                try
                {
                    tcs.SetException(new RequestTimeoutExcption("Request timeout.", request));
                }
                catch { }
            });

            request.Id = serialNumber;
            request.TaskCompletionSource = tcs;
            request.CancellationTokenSource = cts;

            LogsManager.Debug($"Create request. RequestId={request.Id}");

            return request;
        }

        /// <summary>
        /// 请求完成
        /// </summary>
        /// <param name="requestId">RequestId</param>
        /// <param name="result">承载请求响应的消息对象</param>
        public void CompleteRequest(string requestId, DotNettyData result)
        {
            if (!requestList.TryRemove(requestId, out RequestInfo request))
            {
                return;
            }

            try
            {
                request.CancellationTokenSource.Dispose();
                request.TaskCompletionSource.SetResult(result);
                LogsManager.Debug($"Request completed. Request={request.Id}");
            }
            catch (Exception ex)
            {
                LogsManager.Error(ex, $"Request has error. ExceptionMessage={ex.Message}");
            }
        }
    }


    /// <summary>
    /// 请求信息对象
    /// </summary>
    public class RequestInfo
    {
        /// <summary>
        /// RequestId
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// TaskCompletionSource
        /// </summary>
        public TaskCompletionSource<DotNettyData> TaskCompletionSource { get; set; }

        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// Task
        /// </summary>
        public Task<DotNettyData> Task { get { return TaskCompletionSource.Task; } }
    }

    /// <summary>
    /// 请求超时异常
    /// </summary>
    public class RequestTimeoutExcption : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="request">请求信息对象</param>
        public RequestTimeoutExcption(string message, RequestInfo request) : base(message)
        {
            this.Request = request;
        }

        /// <summary>
        /// 请求信息对象
        /// </summary>
        public RequestInfo Request { get; }
    }
}

