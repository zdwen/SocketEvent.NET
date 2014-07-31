using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client.Models.Enums;
using SocketIO.Client.Models.Entities.Messages.Acknownledge;
using SocketIO.Client.Models.Entities.Messages.Event;
using SocketIO.Client.Models.Enums.Acknownledge;

namespace SocketIO.Client.Models.Entities
{
    public class MessageSiocAck : MessageSioc, IMessageReceived, IMessageSend
    {
        ///【闻祖东 2014-7-30-154222】形如6:::1+[{"requestId":"c5670571-f203-417d-a3f4-15cc85b56864","status":"SUCCESS"}]
        ///6:::2+[{"requestId":"a75828e0-afc0-4764-92df-cbacd9fe2927","status":"SUCCESS"}]
        ///其中的guid为发送出去的Event的requestID；1+中的1为发送出去的Event的AckID。
        public override MessageType MessageType { get { return MessageType.Ack; } }
        public int AckId { get; set; }
        public List<AcknowledgeItem> EventInfos { get; set; }
        public string RawMessage { get; set; }

        public MessageSiocAck(string rawMessage)
        {
            RawMessage = rawMessage;
            GenerateProperties();
        }

        public MessageSiocAck(MessageSiocEvent<EventItemReceived> msgEvent)
        {
            AckId = msgEvent.AckId;
            EventInfos = new List<AcknowledgeItem>();
            foreach (EventItem item in msgEvent.EventInfo.Args)
            {
                EventInfos.Add(new AcknowledgeItem()
                {
                    RequestId = item.RequestId,
                    Status = Status.Success,
                    Error = null,
                });
            }
        }

        public void GenerateProperties()
        {
            string[] sInfos = RawMessage.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);

            string[] sAckAndEventInfos = sInfos[1].Split('+');
            AckId = Convert.ToInt32(sAckAndEventInfos[0]);
            EventInfos = CU.JsonDeserialize<List<AcknowledgeItem>>(sAckAndEventInfos[1]);
        }

        public string String4Sent
        {
            get { return string.Format("6:::{0}+{1}", AckId, CU.JsonSerialize(EventInfos)); }
        }
    }
}
