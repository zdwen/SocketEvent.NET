using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SocketIOClient.Messages
{
    public sealed class MessageSiocAck : MessageSioc
    {
        private static Regex reAckComplex = new Regex(@"^\[(?<payload>.*)\]$");

        public Action<dynamic> Callback;

        public MessageSiocAck()
            : base()
        {
            MessageType = SocketIOMessageTypes.ACK;
        }

        public static MessageSiocAck Deserialize(string rawMessage)
        {
            MessageSiocAck msg = new MessageSiocAck() { RawMessage = rawMessage };
            //  '6:::' [message id] '+' [data]
            //   6:::4
            //	 6:::4+["A","B"]
            string[] args = rawMessage.Split(_SplitChars, 4);
            if (args.Length == 4)
            {
                msg.Endpoint = args[2];
                string[] parts = args[3].Split(new char[] { '+' });
                if (parts.Length > 1)
                {
                    int id;
                    if (int.TryParse(parts[0], out id))
                    {
                        msg.AckId = id;
                        msg.MessageText = parts[1];
                        Match payloadMatch = reAckComplex.Match(msg.MessageText);

                        if (payloadMatch.Success)
                        {
                            msg.Json = new JsonEncodedEventMessage();
                            msg.Json.Args = new string[] { payloadMatch.Groups["payload"].Value };
                        }
                    }
                }
            }

            return msg;
        }

        public override string Encoded
        {
            get
            {
                int msgId = (int)MessageType;

                return AckId.HasValue
                    ? string.Format("{0}:::{1}+{2}", msgId, AckId ?? -1, MessageText)
                    : string.Format("{0}::{1}:{2}", msgId, Endpoint, MessageText);
            }
        }
    }
}
