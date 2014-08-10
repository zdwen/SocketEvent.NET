using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client.Models.Enums;

namespace SocketIO.Client.Models.Entities.Message
{
    public abstract class MessageSioc
    {
        public abstract MessageType MessageType { get; }

        //public static MessageSioc CreateMessage(string rawMessage)
        //{
        //    int iMsgType = Convert.ToInt32(rawMessage.Split(':')[0]);
        //    MessageType msgType = CU.Convert2Enum(iMsgType, MessageType.Noop);
        //    switch (msgType)
        //    {
        //        case MessageType.Connect:
        //            return new MessageSiocConnect();
        //        case MessageType.Heartbeat:
        //            return new MessageSiocHeartbeat();
        //        case MessageType.Event:
        //            return new MessageSiocEvent<EventItemReceived>(rawMessage);
        //        case MessageType.Ack:
        //            return new MessageSiocAck(rawMessage);
        //        case MessageType.Disconnect:
        //        case MessageType.Message:
        //        case MessageType.JSONMessage:
        //        case MessageType.Error:
        //        case MessageType.Noop:
        //        default:
        //            string sExceptionMsg = string.Format("【闻祖东 2014-7-30-164001】尚未实现这种消息类型。{0}，RawMessage为：【{1}】", msgType, rawMessage);
        //            CU.Log(sExceptionMsg);
        //            throw new Exception(sExceptionMsg);
        //    }
        //}
    }
}
