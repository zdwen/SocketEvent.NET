using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SocketIOClient.Messages
{
    public class MessageSiocJson : MessageSioc
    {
        public void SetMessage(object value)
        {
            MessageText = JsonConvert.SerializeObject(value, Formatting.None);
        }

        public virtual T Message<T>()
        {
            return JsonConvert.DeserializeObject<T>(MessageText);
        }

        public MessageSiocJson()
        {
            MessageType = SocketIOMessageTypes.JSONMessage;
        }

        public MessageSiocJson(object jsonObject, int? ackId = null, string endpoint = null)
            : this()
        {
            AckId = ackId;
            Endpoint = endpoint;
            MessageText = JsonConvert.SerializeObject(jsonObject, Formatting.None);
        }

        public static MessageSiocJson Deserialize(string rawMessage)
        {
            MessageSiocJson jsonMsg = new MessageSiocJson();
            //  '4:' [message id ('+')] ':' [message endpoint] ':' [json]
            //   4:1::{"a":"b"}
            jsonMsg.RawMessage = rawMessage;

            string[] args = rawMessage.Split(_SplitChars, 4); // limit the number of '

            if (args.Length == 4)
            {
                int id;
                if (int.TryParse(args[1], out id))
                    jsonMsg.AckId = id;

                jsonMsg.Endpoint = args[2];
                jsonMsg.MessageText = args[3];
            }

            return jsonMsg;
        }
    }
}
