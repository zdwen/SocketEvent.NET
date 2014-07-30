using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client.Models.Enums;

namespace SocketIO.Client.Models.Entities
{
    class MessageSiocHeartbeat : MessageSioc, IMessageSend
    {
        //【闻祖东 2014-7-30-154056】固定的2::
        public override MessageType MessageType { get { return MessageType.Heartbeat; } }
        public string String4Sent { get { return "2::"; } }
    }
}
