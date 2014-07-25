using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
    public class MessageSiocHeartbeat : MessageSioc
    {
        public MessageSiocHeartbeat()
        {
            MessageType = SocketIOMessageTypes.Heartbeat;
        }

        public override string Encoded { get { return "2::"; } }
    }
}
