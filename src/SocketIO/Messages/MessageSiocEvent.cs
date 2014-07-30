using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace SocketIOClient.Messages
{
    public class MessageSiocEvent : MessageSioc
    {
        static object _ackIDLock = new object();
        static int _ackID = 0;

        public Action<dynamic> Callback;

        public MessageSiocEvent()
        {
            MessageType = SocketIOMessageTypes.Event;
        }

        public MessageSiocEvent(string eventName, object jsonObject, string endpoint = "", Action<dynamic> callBack = null)
            : this()
        {
            Callback = callBack;
            Endpoint = endpoint;

            if (callBack != null)
                AckId = GenNextAckID();

            Json = new JsonEncodedEventMessage(eventName, jsonObject);
            MessageText = Json.ToJsonString();
        }

        public static MessageSiocEvent Deserialize(string rawMessage)
        {
            MessageSiocEvent evtMsg = new MessageSiocEvent();
            //  '5:' [message id ('+')] ':' [message endpoint] ':' [json encoded event]
            //   5:1::{"a":"b"}
            evtMsg.RawMessage = rawMessage;
            string[] args = rawMessage.Split(_SplitChars, 4); // limit the number of pieces
            if (args.Length == 4)
            {
                int id;
                if (int.TryParse(args[1].Replace("+", ""), out id))
                    evtMsg.AckId = id;

                evtMsg.Endpoint = args[2];
                evtMsg.MessageText = args[3];

                if (!string.IsNullOrEmpty(evtMsg.MessageText) && evtMsg.MessageText.Contains("name") && evtMsg.MessageText.Contains("args"))
                {
                    evtMsg.Json = JsonEncodedEventMessage.Deserialize(evtMsg.MessageText);
                    evtMsg.Event = evtMsg.Json.Name;
                }
                else
                    evtMsg.Json = new JsonEncodedEventMessage();
            }

            return evtMsg;
        }

        public override string Encoded
        {
            get
            {
                int msgId = (int)MessageType;

                return !AckId.HasValue
                    ? string.Format("{0}::{1}:{2}", msgId, Endpoint, MessageText)
                    : Callback == null
                        ? string.Format("{0}:{1}:{2}:{3}", msgId, AckId ?? -1, Endpoint, MessageText)
                        : string.Format("{0}:{1}+:{2}:{3}", msgId, AckId ?? -1, Endpoint, MessageText);
            }
        }

        static int GenNextAckID()
        {
            lock (_ackIDLock)
                return ++_ackID;
        }
    }
}
