using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SocketIOClient.Messages;

namespace SocketIOClient.Eventing
{
    /// <summary>
    /// 【闻祖东 2014-7-25-180804】？事件注册管理器？
    /// </summary>
    public class RegistrationManager : IDisposable
    {
        ConcurrentDictionary<int, Action<dynamic>> _callBackRegistry;
        ConcurrentDictionary<string, Action<IMessageSioc>> _eventNameRegistry;

        public RegistrationManager()
        {
            _callBackRegistry = new ConcurrentDictionary<int, Action<dynamic>>();
            _eventNameRegistry = new ConcurrentDictionary<string, Action<IMessageSioc>>();
        }

        public void AddCallBack(MessageSiocEvent eventMessage)
        {
            _callBackRegistry.AddOrUpdate(eventMessage.AckId.Value, eventMessage.Callback, (key, oldValue) => eventMessage.Callback);
        }

        /// <summary>
        /// TODO【闻祖东 2014-7-29-150240】？？这个地方最后还是改成异步调用
        /// </summary>
        /// <param name="ackId"></param>
        /// <param name="value"></param>
        public void InvokeCallBack(int? ackId, JsonEncodedEventMessage value)
        {
            Action<dynamic> target = null;
            if (ackId.HasValue && _callBackRegistry.TryRemove(ackId.Value, out target))
                target.Invoke(value);
        }

        public void AddOnEvent(string eventName, Action<IMessageSioc> callback)
        {
            _eventNameRegistry.AddOrUpdate(eventName, callback, (key, oldValue) => callback);
        }

        /// <summary>
        /// If eventName is found, Executes Action delegate<typeparamref name="T"/> asynchronously
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void InvokeOnEvent(IMessageSioc value)
        {
            string eventName = value.Event;
            if (!string.IsNullOrWhiteSpace(value.Endpoint))
                eventName = string.Format("{0}::{1}", value.Event, value.Endpoint);

            if (_eventNameRegistry.ContainsKey(eventName))
            {
                ///TODO【闻祖东 2014-7-29-115326】代码注释说这个地方需要异步，但是实际上是在执行同步？
                _eventNameRegistry[eventName].Invoke(value);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _callBackRegistry.Clear();
            _eventNameRegistry.Clear();
        }
    }
}
