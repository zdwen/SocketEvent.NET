using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client.Models.Entities.Messages.Event;

namespace TestForm.Entities.DTOs.Operations
{
    public class DtoSocketIoEventBase : IEventSummary
    {
        public string EventName { get; set; }
        public string ClientID { get; set; }
        public string RequestID { get; set; }

        public string Name { get { return EventName; } }
    }
}
