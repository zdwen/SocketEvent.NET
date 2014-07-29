using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Models.Enums
{
    public enum SocketIOEvent
    {
        Message,
        Connect,
        Disconnect,
        Open,
        Close,
        Error,
        Retry,
        Reconnect,
    }
}
