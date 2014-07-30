using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client.Models.Enums;
using SocketIO.Client.Models.Entities.Messages.Event;

namespace SocketIO.Client.Models.Entities
{
    public class MessageSiocEvent : MessageSioc, IMessageSend, IMessageReceived
    {
        ///5:1+::{"name":"subscribe","args":[{"event":"PriceChanged","requestId":"31e1860b-2986-4dd8-92d2-c42424835afd","senderId":"02ab3d36-0866-4dd8-81b4-cf1bb1014e37"}]}
        ///5:1+::{"name":"subscribe","args":[{"event":"PriceChanged","requestId":"c5670571-f203-417d-a3f4-15cc85b56864","senderId":"MerchantServiceClient"}]}
        ///5:1+::{"name":"PublishSalesState","args":[{"requestId":"f8e72612-d404-4950-9673-0b3dcddd324a","event":"PublishSalesState","args":{"ListingSku":"5100718"}}]}
        public override MessageType MessageType { get { return MessageType.Event; } }
        public int AckId { get; private set; }
        public EventInfo EventInfo { get; set; }
        public string RawMessage { get; set; }

        public string String4Sent
        {
            get
            {
                ///5:1+::{'name':'subscribe','args':[{'event':'PriceChanged','requestId':'c5670571-f203-417d-a3f4-15cc85b56864','senderId':'MerchantServiceClient'}]}
                return string.Format("5:{0}+::{1}", AckId, CU.JsonSerialize(EventInfo));
            }
        }

        /// <summary>
        /// 【闻祖东 2014-7-30-181730】由Client端构造。
        /// </summary>
        /// <param name="ackId"></param>
        public MessageSiocEvent(int ackId)
        {
            AckId = ackId;
        }

        /// <summary>
        /// 【闻祖东 2014-7-30-181711】由来自服务端的字符串构造。
        /// </summary>
        /// <param name="rawMessage"></param>
        public MessageSiocEvent(string rawMessage)
        {
            RawMessage = rawMessage;
            GenerateProperties();
        }

        public void GenerateProperties()
        {
            string[] sInfos = RawMessage.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            AckId = Convert.ToInt32(sInfos[1].Replace("+", string.Empty));
            EventInfo = CU.JsonDeserialize<EventInfo>(sInfos[3]);
        }
    }
}
