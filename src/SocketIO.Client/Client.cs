using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using System.Collections.Concurrent;
using System.Net;
using SocketIO.Client.Models.Entities;
using SocketIO.Client.Models.Enums;
using SocketIO.Client.Models.Entities.Messages.Event;

namespace SocketIO.Client
{
    /// <summary>
    /// TODO【闻祖东 2014-7-30-112753】
    /// 1.Reconnect的业务未实现；
    /// 2.暂时未考虑Authentication的业务，故Headers的相应业务也删除；
    /// 3.
    /// </summary>
    public class Client// : IDisposable
    {
        const WebSocketVersion _dftWsVersion = WebSocketVersion.Rfc6455;
        int _ackID;

        /// <summary>
        /// 【闻祖东 2014-7-25-170952】等待出去的消息队列。
        /// 其实是应用于当前的场景是，基本上传递的消息都是字符串，所以用的是一个string的集合。
        /// </summary>
        BlockingCollection<string> _outboundQueue;
        Uri _uri;
        WebSocket _wsClient;
        string _sessionID;

        public WebSocketState ReadyState { get { return _wsClient != null ? _wsClient.State : WebSocketState.None; } }
        //public 

        public Client(string url)
        {
            _uri = new Uri(url);
            Connect();
            _outboundQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());
            //dequeuOutBoundMsgTask = Task.Factory.StartNew(() => DequeueOutboundMessages(), TaskCreationOptions.LongRunning);
        }

        void Connect()
        {
            GetSessionID();

            string wsScheme = (_uri.Scheme == Uri.UriSchemeHttps ? "wss" : "ws");
            _wsClient = new WebSocket(string.Format("{0}://{1}:{2}/socket.io/1/websocket/{3}", wsScheme, _uri.Host, _uri.Port, _sessionID), string.Empty, _dftWsVersion);

            _wsClient.EnableAutoSendPing = false; // #4 tkiley: Websocket4net client library initiates a websocket heartbeat, causes delivery problems

            _wsClient.MessageReceived += wsClient_MessageReceived;
            //_wsClient.Error += wsClient_Error;
            //_wsClient.Closed += wsClient_Closed;

            _wsClient.Open();
        }

        void wsClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageSioc msg = MessageSioc.CreateMessage(e.Message);
            Console.WriteLine("{0}-{1}\t{2}", DateTime.Now, msg.MessageType, e.Message);
            switch (msg.MessageType)
            {
                case MessageType.Disconnect:
                    break;
                case MessageType.Connect:
                    break;
                case MessageType.Heartbeat:
                    Response4Heartbeat();
                    break;
                case MessageType.Message:
                    break;
                case MessageType.JSONMessage:
                    break;
                case MessageType.Event:
                    break;
                case MessageType.Ack:
                    break;
                case MessageType.Error:
                    break;
                case MessageType.Noop:
                    break;
                default:
                    break;
            }
            //IMessageSioc iMsg = MessageSioc.Factory(e.Message);

            //Console.WriteLine("{0}-{1}-{2}\te.Message={3}", DateTime.Now, iMsg.GetType().Name, iMsg.Event, e.Message);
            //Log("{0}-{1}-{2}\te.Message:\t{3}", DateTime.Now, iMsg.GetType().Name, iMsg.Event, e.Message);

            //switch (iMsg.MessageType)
            //{
            //    case SocketIOMessageTypes.Disconnect:
            //        OnMessageEvent(iMsg);
            //        if (string.IsNullOrWhiteSpace(iMsg.Endpoint)) // Disconnect the whole socket
            //            Close();
            //        break;
            //    case SocketIOMessageTypes.Heartbeat:
            //        OnHeartBeatTimerCallback(null);
            //        break;
            //    case SocketIOMessageTypes.Connect:
            //    case SocketIOMessageTypes.Message:
            //    case SocketIOMessageTypes.JSONMessage:
            //    case SocketIOMessageTypes.Event:
            //    case SocketIOMessageTypes.Error:
            //        Console.WriteLine("wsClient_MessageReceived里面执行:{0}", iMsg.MessageType);
            //        OnMessageEvent(iMsg);
            //        break;
            //    case SocketIOMessageTypes.ACK:
            //        _regMnger.InvokeCallBack(iMsg.AckId, iMsg.Json);
            //        break;
            //    default:
            //        Trace.WriteLine("unknown wsClient message Received...");
            //        break;
            //}
        }

        void GetSessionID()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string sReturn = client.DownloadString(string.Format("{0}://{1}:{2}/socket.io/1/{3}", _uri.Scheme, _uri.Host, _uri.Port, _uri.Query)); // #5 tkiley: The uri.Query is available in socket.io's handshakeData object during authorization
                    // 13052140081337757257:15:25:websocket,htmlfile,xhr-polling,jsonp-polling
                    _sessionID = sReturn.Split(':')[0];
                }
                catch (Exception ex)
                {
                    ///TODO【闻祖东 2014-7-30-115234】这里面的异常相信处理之后参照原来的。
                }
            }
        }

        /// <summary>
        /// 【闻祖东 2014-7-30-180939】如若不给服务器以心跳回应，服务器将不会推送消息到此Client。
        /// </summary>
        void Response4Heartbeat()
        {
            _wsClient.Send(new MessageSiocHeartbeat().String4Sent);
        }

        public void SendEvent(EventInfo eventInfo)
        {
            MessageSiocEvent msg = new MessageSiocEvent(++_ackID)
            {
                EventInfo = eventInfo,
            };

            _wsClient.Send(msg.String4Sent);
        }
    }
}
