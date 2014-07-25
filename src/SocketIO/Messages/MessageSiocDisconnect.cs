using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
    /// <summary>
    /// Signals disconnection. If no endpoint is specified, disconnects the entire socket.
    /// </summary>
    /// <remarks>Disconnect a socket connected to the /test endpoint:
    ///		0::/test
    /// </remarks>
    public class MessageSiocDisconnect : MessageSioc
    {
        public override string Event { get { return "disconnect"; } }

        public MessageSiocDisconnect()
            : base()
        {
            MessageType = SocketIOMessageTypes.Disconnect;
        }

        public MessageSiocDisconnect(string endPoint)
            : this()
        {
            Endpoint = endPoint;
        }

        public static MessageSiocDisconnect Deserialize(string rawMessage)
        {
            MessageSiocDisconnect msg = new MessageSiocDisconnect();
            //  0::
            //  0::/test
            msg.RawMessage = rawMessage;

            string[] args = rawMessage.Split(_SplitChars, 3);
            if (args.Length == 3 && !string.IsNullOrWhiteSpace(args[2]))
                msg.Endpoint = args[2];

            return msg;
        }

        public override string Encoded
        {
            get
            {
                return string.Format("0::{0}", Endpoint);
            }
        }
    }
}
