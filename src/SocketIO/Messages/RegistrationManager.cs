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

        public void AddCallBack(IMessageSioc message)
        {
            MessageSiocEvent eventMessage = message as MessageSiocEvent;
            if (eventMessage != null)
                _callBackRegistry.AddOrUpdate(eventMessage.AckId.Value, eventMessage.Callback, (key, oldValue) => eventMessage.Callback);
        }

        public void AddCallBack(int ackId, Action<dynamic> callback)
        {
            _callBackRegistry.AddOrUpdate(ackId, callback, (key, oldValue) => callback);
        }

        public void InvokeCallBack(int? ackId, string value)
        {
            Action<dynamic> target = null;
            if (ackId.HasValue)
            {
                if (_callBackRegistry.TryRemove(ackId.Value, out target)) // use TryRemove - callbacks are one-shot event registrations
                {
                    target.BeginInvoke(value, target.EndInvoke, null);
                }
            }
        }

        public void InvokeCallBack(int? ackId, JsonEncodedEventMessage value)
        {
            Action<dynamic> target = null;
            if (ackId.HasValue)
            {
                if (_callBackRegistry.TryRemove(ackId.Value, out target))
                {
                    target.Invoke(value);
                    //target.BeginInvoke(value, target.EndInvoke, null);
                }
            }
        }

        public void AddOnEvent(string eventName, Action<IMessageSioc> callback)
        {
            _eventNameRegistry.AddOrUpdate(eventName, callback, (key, oldValue) => callback);
        }

        public void AddOnEvent(string eventName, string endPoint, Action<IMessageSioc> callback)
        {
            _eventNameRegistry.AddOrUpdate(string.Format("{0}::{1}", eventName, endPoint), callback, (key, oldValue) => callback);
        }

        /// <summary>
        /// If eventName is found, Executes Action delegate<typeparamref name="T"/> asynchronously
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InvokeOnEvent(IMessageSioc value)
        {
            bool foundEvent = false;
            try
            {
                Action<IMessageSioc> target;

                string eventName = value.Event;
                if (!string.IsNullOrWhiteSpace(value.Endpoint))
                    eventName = string.Format("{0}::{1}", value.Event, value.Endpoint);

                if (_eventNameRegistry.TryGetValue(eventName, out target)) // use TryGet - do not destroy event name registration
                {
                    foundEvent = true;
                    target.Invoke(value);
                    //target.BeginInvoke(value, target.EndInvoke, null);
                    //Trace.WriteLine(string.Format("webSocket_{0}: {1}", value.Event, value.MessageText));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception on InvokeOnEvent: " + ex.Message);
            }

            return foundEvent;
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
