using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Messages.Event
{
    public class EventSummary<T> : IEventSummary
    {
        /// <summary>
        /// 【闻祖东 2014-8-10-111618】此属性对应SocketIo服务端的Socket.on和socket.emit的第一个参数（“事件名”）。
        /// </summary>
        public string Name { get; set; }
        public T Args { get; set; }
    }
}
