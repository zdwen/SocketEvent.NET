using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Messages.Event
{
    public class EventItemSent : EventItem
    {
        public string SenderId { get; set; }
    }
}
