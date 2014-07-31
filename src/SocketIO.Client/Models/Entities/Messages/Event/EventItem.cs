using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Messages.Event
{
    /// <summary>
    /// 【闻祖东 2014-7-31-104758】SocketIO所需要的这三个属性还缺一不可。
    /// </summary>
    public abstract class EventItem
    {
        public string Event { get; set; }
        public string RequestId { get; set; }
    }
}
