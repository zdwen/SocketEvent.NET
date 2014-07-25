using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
	/// <summary>
	/// Defined as No operation. Used for example to close a poll after the polling duration times out.
	/// </summary>
    public class MessageSiocNoop : MessageSioc
    {
        public MessageSiocNoop()
        {
            MessageType = SocketIOMessageTypes.Noop;
        }

        public static MessageSiocNoop Deserialize(string rawMessage)
        {
			return new MessageSiocNoop();
        }
    }
}
