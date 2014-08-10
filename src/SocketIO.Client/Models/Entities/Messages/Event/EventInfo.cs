using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Messages.Event
{
    public class EventInfo<T> : IEventSummary where T : EventItem, new()
    {
        public string Name { get; set; }
        public List<T> Args { get; set; }
    }
}
