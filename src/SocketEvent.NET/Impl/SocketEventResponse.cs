using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketEvent.Impl
{
    public class SocketEventResponse
    {
        public string RequestId { get; set; }
        public RequestResult Status { get; set; }
        public ServerError Error { get; set; }
    }
}
