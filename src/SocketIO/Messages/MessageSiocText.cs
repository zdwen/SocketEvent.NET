using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIOClient;
using System.Text.RegularExpressions;

namespace SocketIOClient.Messages
{
    public class MessageSiocText : MessageSioc
    {
        public override string Event { get { return "message"; } }

        public MessageSiocText()
        {
            MessageType = SocketIOMessageTypes.Message;
        }

        public MessageSiocText(string textMessage)
            : this()
        {
            MessageText = textMessage;
        }

        public static MessageSiocText Deserialize(string rawMessage)
        {
            MessageSiocText msg = new MessageSiocText();
            //  '3:' [message id ('+')] ':' [message endpoint] ':' [data]
            //   3:1::blabla
            msg.RawMessage = rawMessage;

            string[] args = rawMessage.Split(_SplitChars, 4);
            if (args.Length == 4)
            {
                int id;
                if (int.TryParse(args[1], out id))
                    msg.AckId = id;

                msg.Endpoint = args[2];
                msg.MessageText = args[3];
            }
            else
                msg.MessageText = rawMessage;

            return msg;
        }
    }
}
