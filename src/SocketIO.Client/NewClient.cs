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
using System.Threading;
using SuperSocket.ClientEngine;
using System.Threading.Tasks;

namespace SocketIO.Client
{
    /// <summary>
    /// TODO【闻祖东 2014-7-30-112753】
    /// 1.Reconnect的业务未实现；
    /// 2.暂时未考虑Authentication的业务，故Headers的相应业务也删除；
    /// 3.
    /// </summary>
    public class NewClient : IDisposable
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
        ManualResetEvent _switch4Communication;

        public event Action<EventInfo<EventItemReceived>> EventArrived;

        public NewClient(string url)
        {
            _switch4Communication = new ManualResetEvent(false);
            _uri = new Uri(url);
            Connect();
            _outboundQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());
            //dequeuOutBoundMsgTask = Task.Factory.StartNew(() => DequeueOutboundMessages(), TaskCreationOptions.LongRunning);
        }

        void Connect()
        {
            _switch4Communication.Reset();
            GetSessionID();

            string wsScheme = (_uri.Scheme == Uri.UriSchemeHttps ? "wss" : "ws");
            _wsClient = new WebSocket(string.Format("{0}://{1}:{2}/socket.io/1/websocket/{3}", wsScheme, _uri.Host, _uri.Port, _sessionID), string.Empty, _dftWsVersion);

            _wsClient.EnableAutoSendPing = false; // #4 tkiley: Websocket4net client library initiates a websocket heartbeat, causes delivery problems

            _wsClient.Opened += _wsClient_Opened;
            _wsClient.MessageReceived += wsClient_MessageReceived;
            _wsClient.Error += _wsClient_Error;
            _wsClient.Closed += _wsClient_Closed;

            _wsClient.Open();
        }

        void CloseWebSocketClient()
        {
            if (_wsClient.State == WebSocketState.Connecting || _wsClient.State == WebSocketState.Open)
                _wsClient.Close();

            _wsClient.Closed -= _wsClient_Closed;
            _wsClient.MessageReceived -= wsClient_MessageReceived;
            _wsClient.Error -= _wsClient_Error;
            _wsClient.Opened -= _wsClient_Opened;

            _wsClient = null;
        }

        void _wsClient_Closed(object sender, EventArgs e)
        {
            _switch4Communication.Reset();
        }

        void _wsClient_Error(object sender, ErrorEventArgs e)
        {
            _switch4Communication.Reset();
        }

        void _wsClient_Opened(object sender, EventArgs e)
        {
            CU.Log("与服务端的连接打开成功。");
            ///【闻祖东 2014-7-31-110120】设置为终止状态，那么所有的连接请求可以直接不被阻塞的执行。
            _switch4Communication.Set();
        }

        void wsClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Action action = () =>
            {
                CU.Log("wsClient_MessageReceived,e.Message=\t{0}", e.Message);
                MessageSioc msg = MessageSioc.CreateMessage(e.Message);

                switch (msg.MessageType)
                {
                    case MessageType.Heartbeat:
                        Response4Heartbeat();
                        break;
                    case MessageType.Event:
                        Response4Event((MessageSiocEvent<EventItemReceived>)msg);
                        break;
                    case MessageType.Disconnect:
                    case MessageType.Connect:
                    case MessageType.Message:
                    case MessageType.JSONMessage:
                    case MessageType.Ack:
                    case MessageType.Error:
                    case MessageType.Noop:
                    default:
                        break;
                }
            };

            ///【闻祖东 2014-7-31-173117】相当于总的开关处控制每个消息到达的时候都是新开启的线程去处理业务，其他地方不用
            ///再考虑多开线程的问题。
            Task.Factory.StartNew(action);
        }

        void GetSessionID()
        {
            using (WebClient client = new WebClient())
            {
                string sReturn = client.DownloadString(string.Format("{0}://{1}:{2}/socket.io/1/{3}", _uri.Scheme, _uri.Host, _uri.Port, _uri.Query)); // #5 tkiley: The uri.Query is available in socket.io's handshakeData object during authorization
                // 13052140081337757257:15:25:websocket,htmlfile,xhr-polling,jsonp-polling
                _sessionID = sReturn.Split(':')[0];
            }
        }

        /// <summary>
        /// 【闻祖东 2014-7-30-180939】如若不给服务器以心跳回应，服务器将不会推送消息到此Client。
        /// </summary>
        void Response4Heartbeat()
        {
            SendMessage(new MessageSiocHeartbeat());
        }

        void Response4Event(MessageSiocEvent<EventItemReceived> msgEvent)
        {
            SendMessage(new MessageSiocAck(msgEvent));

            if (EventArrived != null)
                EventArrived.Invoke(msgEvent.EventInfo);
        }

        public void SendEvent(EventInfo<EventItemSent> eventInfo)
        {
            MessageSiocEvent<EventItemSent> msg = new MessageSiocEvent<EventItemSent>(++_ackID)
            {
                EventInfo = eventInfo,
            };

            SendMessage(msg);
        }







        void SendMessage(IMessageSend msg)
        {
            _switch4Communication.WaitOne();

            CU.Log("_wsClient.Send-{0}", msg.String4Sent);
            _wsClient.Send(msg.String4Sent);
        }














        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseWebSocketClient();
                _switch4Communication.Dispose();
            }
        }
    }
}
