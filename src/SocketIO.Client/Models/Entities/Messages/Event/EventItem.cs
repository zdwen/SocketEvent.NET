using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Messages.Event
{
    public class EventItem
    {
        public string Event { get; set; }
        public string RequestId { get; set; }
        public string SenderId { get; set; }
    }
}
