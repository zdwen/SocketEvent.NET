using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketEvent.Impl
{
    public class SocketEventRequest
    {
        public string EventName { get; set; }
        public string RequestId { get; set; }
        public dynamic Args { get; set; }
    }
}
