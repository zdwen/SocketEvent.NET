using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SocketIOClient.Eventing;
using SocketIOClient.Messages;
using WebSocket4Net;
using System.ComponentModel;
using SuperSocket.ClientEngine;
using System.Runtime.Remoting.Messaging;

namespace SocketIOClient
{
    /// <summary>
    /// Class to emulate socket.io javascript client capabilities for .net classes
    /// 【闻祖东 2014-7-25-164149】SocketIoClient，但还不是直接用于提供业务意义的Client。
    /// </summary>
    /// <exception cref = "ArgumentException">Connection for wss or https urls</exception>  
    public class Client : IDisposable
    {
        const WebSocketVersion _dftSocketVersion = WebSocketVersion.Rfc6455;
        readonly static object _padLock = new object(); // allow one connection attempt at a time

        Timer _heartBeatTimer;
        /// <summary>
        /// 【闻祖东 2014-7-25-171905】处理出列的任务
        /// </summary>
        Task dequeuOutBoundMsgTask;
        /// <summary>
        /// 【闻祖东 2014-7-25-170952】等待出去的消息队列？？
        /// </summary>
        BlockingCollection<string> outboundQueue;
        /// <summary>
        /// 【闻祖东 2014-7-25-171617】实际重连次数。
        /// </summary>
        int retryConnectionCount = 0;

        /// <summary>
        /// Uri of Websocket server
        /// </summary>
        Uri _uri;
        /// <summary>
        /// Underlying WebSocket implementation
        /// 【闻祖东 2014-7-25-172017】为什么这个变量要命名为Client，不是很明白。
        /// </summary>
        WebSocket _wsClient;
        /// <summary>
        /// RegistrationManager for dynamic events
        /// </summary>
        RegistrationManager regMnger;  // allow registration of dynamic events (event names) for client actions
        /// <summary>
        /// By Default, use WebSocketVersion.Rfc6455
        /// </summary>
        WebSocketVersion SocketVersion { get; set; }

        public event EventHandler<MessageEventArgs> Message;
        public event EventHandler ConnectionRetryAttempt;
        public event EventHandler HeartBeatTimerEvent;
        /// <summary>
        /// Happens when reconnected.
        /// </summary>
        public event EventHandler ConnectionReconnect;
        public event EventHandler<EventArgsSiocError> Error;

        /// <summary>
        /// ResetEvent for Outbound MessageQueue Empty Event - all pending messages have been sent
        /// </summary>
        public ManualResetEvent MessageQueueEmptyEvent = new ManualResetEvent(true);

        /// <summary>
        /// Connection Open Event
        /// </summary>
        public ManualResetEvent ConnectionOpenEvent = new ManualResetEvent(false);

        /// <summary>
        /// Number of reconnection attempts before raising SocketConnectionClosed event - (default = 3)
        /// </summary>
        [DefaultValue(3)]
        public int RetryConnectionAttempts { get; set; }

        /// <summary>
        /// Value of the last error message text  
        /// </summary>
        public string LastErrorMessage = string.Empty;

        /// <summary>
        /// Represents the initial handshake parameters received from the socket.io service (SID, HeartbeatTimeout etc)
        /// </summary>
        public SocketIOHandshake HandShake { get; private set; }

        public bool IsConnected { get { return ReadyState == WebSocketState.Open; } }

        /// <summary>
        /// Connection state of websocket client: None, Connecting, Open, Closing, Closed
        /// </summary>
        public WebSocketState ReadyState
        {
            get
            {
                return _wsClient != null
                    ? _wsClient.State
                    : WebSocketState.None;
            }
        }

        public Client(string url, NameValueCollection headers = null, WebSocketVersion socketVersion = _dftSocketVersion)
        {
            _uri = new Uri(url);

            HandShake = new SocketIOHandshake(headers);
            SocketVersion = socketVersion;

            regMnger = new RegistrationManager();
            outboundQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());
            dequeuOutBoundMsgTask = Task.Factory.StartNew(() => DequeueOutboundMessages(), TaskCreationOptions.LongRunning);
        }
        /// <summary>
        /// Initiate the connection with Socket.IO service
        /// </summary>
        public void Connect()
        {
            lock (_padLock)
            {
                if (!(ReadyState == WebSocketState.Connecting || ReadyState == WebSocketState.Open))
                {
                    try
                    {
                        ConnectionOpenEvent.Reset();
                        RequestHandshake(_uri);// perform an initial HTTP request as a new, non-handshaken connection

                        if (string.IsNullOrWhiteSpace(HandShake.SID) || HandShake.HasError)
                        {
                            LastErrorMessage = string.Format("Error initializing handshake with {0}", _uri.ToString());
                            OnErrorEvent(this, new EventArgsSiocError(LastErrorMessage, new Exception()));
                        }
                        else
                        {
                            string wsScheme = (_uri.Scheme == Uri.UriSchemeHttps ? "wss" : "ws");
                            _wsClient = new WebSocket(string.Format("{0}://{1}:{2}/socket.io/1/websocket/{3}", wsScheme, _uri.Host, _uri.Port, HandShake.SID), string.Empty, SocketVersion);

                            _wsClient.EnableAutoSendPing = false; // #4 tkiley: Websocket4net client library initiates a websocket heartbeat, causes delivery problems

                            _wsClient.Opened += wsClient_OpenEvent;
                            _wsClient.MessageReceived += wsClient_MessageReceived;
                            _wsClient.Error += wsClient_Error;
                            _wsClient.Closed += wsClient_Closed;

                            _wsClient.Open();
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(string.Format("Connect threw an exception...{0}", ex.Message));
                        OnErrorEvent(this, new EventArgsSiocError("SocketIO.Client.Connect threw an exception", ex));
                    }
                }
            }
        }

        protected void ReConnect()
        {
            bool connected = false;

            while (!connected && retryConnectionCount < RetryConnectionAttempts)
            {
                retryConnectionCount++;

                OnConnectionRetryAttemptEvent(this, EventArgs.Empty);

                CloseHeartBeatTimer(); // stop the heartbeat time
                CloseWebSocketClient();// stop websocket
                HandShake.ResetConnection();

                Connect();

                connected = ConnectionOpenEvent.WaitOne(4000); // block while waiting for connection
                Trace.WriteLine(string.Format("\tRetry-Connection successful: {0}", connected));
            }

            if (connected)
            {
                OnConnectionReconnectEvent(this, EventArgs.Empty);
                retryConnectionCount = 0;
            }
            else
            {
                Close();
                OnSocketConnectionClosedEvent(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// <para>Asynchronously calls the action delegate on event message notification</para>
        /// <para>Mimicks the Socket.IO client 'socket.on('name',function(data){});' pattern</para>
        /// <para>Reserved socket.io event names available: connect, disconnect, open, close, error, retry, reconnect  </para>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        /// <example>
        /// client.On("testme", (data) =>
        ///    {
        ///        Debug.WriteLine(data.ToJson());
        ///    });
        /// </example>
        public virtual void On(string eventName, Action<IMessageSioc> action)
        {
            regMnger.AddOnEvent(eventName, action);
        }

        public virtual void On(string eventName, string endPoint, Action<IMessageSioc> action)
        {
            regMnger.AddOnEvent(eventName, endPoint, action);
        }

        /// <summary>
        /// <para>Asynchronously sends payload using eventName</para>
        /// <para>payload must a string or Json Serializable</para>
        /// <para>Mimicks Socket.IO client 'socket.emit('name',payload);' pattern</para>
        /// <para>Do not use the reserved socket.io event names: connect, disconnect, open, close, error, retry, reconnect</para>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="payload">must be a string or a Json Serializable object</param>
        /// <remarks>ArgumentOutOfRangeException will be thrown on reserved event names</remarks>
        public void Emit(string eventName, dynamic payload, string endPoint = "", Action<dynamic> callback = null)
        {
            string lceventName = eventName.ToLower();
            IMessageSioc msg = null;
            switch (lceventName)
            {
                case "message":
                    if (payload is string)
                        msg = new MessageSiocText() { MessageText = payload };
                    else
                        msg = new MessageSiocJson(payload);
                    Send(msg);
                    break;
                case "connect":
                case "disconnect":
                case "open":
                case "close":
                case "error":
                case "retry":
                case "reconnect":
                    throw new ArgumentOutOfRangeException(eventName, "Event name is reserved by socket.io, and cannot be used by clients or servers with this message type");
                default:
                    if (!string.IsNullOrWhiteSpace(endPoint) && !endPoint.StartsWith("/"))
                        endPoint = "/" + endPoint;
                    msg = new MessageSiocEvent(eventName, payload, endPoint, callback);
                    if (callback != null)
                        regMnger.AddCallBack(msg);

                    Send(msg);
                    break;
            }
        }

        /// <summary>
        /// <para>Asynchronously sends payload using eventName</para>
        /// <para>payload must a string or Json Serializable</para>
        /// <para>Mimicks Socket.IO client 'socket.emit('name',payload);' pattern</para>
        /// <para>Do not use the reserved socket.io event names: connect, disconnect, open, close, error, retry, reconnect</para>
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="payload">must be a string or a Json Serializable object</param>
        public void Emit(string eventName, dynamic payload)
        {
            Emit(eventName, payload, string.Empty, null);
        }

        /// <summary>
        /// Queue outbound message
        /// </summary>
        /// <param name="msg"></param>
        public void Send(IMessageSioc msg)
        {
            MessageQueueEmptyEvent.Reset();
            outboundQueue.Add(msg.Encoded);
        }

        /// <summary>
        /// if a registerd event name is found, don't raise the more generic Message event
        /// </summary>
        /// <param name="msg"></param>
        void OnMessageEvent(IMessageSioc msg)
        {
            bool skip = false;
            if (!string.IsNullOrEmpty(msg.Event))
                skip = regMnger.InvokeOnEvent(msg); // 

            if (Message != null && !skip)
            {
                Trace.WriteLine(string.Format("webSocket_OnMessage: {0}", msg.RawMessage));
                Message(this, new MessageEventArgs(msg));
            }
        }

        /// <summary>
        /// Close SocketIO4Net.Client and clear all event registrations 
        /// </summary>
        public void Close()
        {
            retryConnectionCount = 0;
            CloseHeartBeatTimer();
            CloseOutboundQueue();
            CloseWebSocketClient();

            regMnger.Dispose();
        }

        void CloseHeartBeatTimer()
        {
            _heartBeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _heartBeatTimer.Dispose();
        }

        void CloseOutboundQueue()
        {
            outboundQueue.CompleteAdding(); // stop adding any more items;
            dequeuOutBoundMsgTask.Wait(700); // wait for dequeue thread to stop
            outboundQueue.Dispose();
        }

        void CloseWebSocketClient()
        {
            if (_wsClient != null)
            {
                // unwire events
                _wsClient.Closed -= wsClient_Closed;
                _wsClient.MessageReceived -= wsClient_MessageReceived;
                _wsClient.Error -= wsClient_Error;
                _wsClient.Opened -= wsClient_OpenEvent;

                if (_wsClient.State == WebSocketState.Connecting || _wsClient.State == WebSocketState.Open)
                {
                    try
                    {
                        _wsClient.Close();
                    }
                    catch { Trace.WriteLine("exception raised trying to close websocket: can safely ignore, socket is being closed"); }
                }

                _wsClient = null;
            }
        }


        // websocket client events - open, messages, errors, closing
        void wsClient_OpenEvent(object sender, EventArgs e)
        {
            _heartBeatTimer = new Timer(OnHeartBeatTimerCallback, new object(), HandShake.HeartbeatInterval, HandShake.HeartbeatInterval);
            ConnectionOpenEvent.Set();

            OnMessageEvent(new MessageSiocEvent() { Event = "open" });
        }

        /// <summary>
        /// Raw websocket messages from server - convert to message types and call subscribers of events and/or callbacks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wsClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            IMessageSioc iMsg = MessageSioc.Factory(e.Message);

            if (iMsg.Event == "responseMsg")
                Trace.WriteLine(string.Format("InvokeOnEvent: {0}", iMsg.RawMessage));

            switch (iMsg.MessageType)
            {
                case SocketIOMessageTypes.Disconnect:
                    OnMessageEvent(iMsg);
                    if (string.IsNullOrWhiteSpace(iMsg.Endpoint)) // Disconnect the whole socket
                        Close();
                    break;
                case SocketIOMessageTypes.Heartbeat:
                    OnHeartBeatTimerCallback(null);
                    break;
                case SocketIOMessageTypes.Connect:
                case SocketIOMessageTypes.Message:
                case SocketIOMessageTypes.JSONMessage:
                case SocketIOMessageTypes.Event:
                case SocketIOMessageTypes.Error:
                    OnMessageEvent(iMsg);
                    break;
                case SocketIOMessageTypes.ACK:
                    regMnger.InvokeCallBack(iMsg.AckId, iMsg.Json);
                    break;
                default:
                    Trace.WriteLine("unknown wsClient message Received...");
                    break;
            }
        }

        /// <summary>
        /// websocket has closed unexpectedly - retry connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wsClient_Closed(object sender, EventArgs e)
        {
            if (retryConnectionCount < RetryConnectionAttempts)
            {
                ConnectionOpenEvent.Reset();
                ReConnect();
            }
            else
            {
                Close();
                OnSocketConnectionClosedEvent(this, EventArgs.Empty);
            }
        }

        void wsClient_Error(object sender, ErrorEventArgs e)
        {
            OnErrorEvent(sender, new EventArgsSiocError("SocketClient error", e.Exception));
        }

        void OnErrorEvent(object sender, EventArgsSiocError e)
        {
            LastErrorMessage = e.Message;
            if (Error != null)
            {
                try
                {
                    Error.Invoke(this, e);
                }
                catch { }
            }
            Trace.WriteLine(string.Format("Error Event: {0}\r\n\t{1}", e.Message, e.Exception));
        }

        void OnSocketConnectionClosedEvent(object sender, EventArgs e)
        {
            Trace.WriteLine("SocketConnectionClosedEvent");
        }

        void OnConnectionRetryAttemptEvent(object sender, EventArgs e)
        {
            if (ConnectionRetryAttempt != null)
            {
                try
                {
                    ConnectionRetryAttempt(sender, e);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
            Trace.WriteLine(string.Format("Attempting to reconnect: {0}", retryConnectionCount));
        }

        void OnConnectionReconnectEvent(object sender, EventArgs e)
        {
            try
            {
                if (ConnectionReconnect != null)
                    ConnectionReconnect(sender, e);
            }
            catch { }
        }

        // Housekeeping
        void OnHeartBeatTimerCallback(object state)
        {
            if (ReadyState == WebSocketState.Open)
            {
                try
                {
                    if (!outboundQueue.IsAddingCompleted)
                    {
                        outboundQueue.Add(new MessageSiocHeartbeat().Encoded);
                        if (HeartBeatTimerEvent != null)
                            HeartBeatTimerEvent.BeginInvoke(this, EventArgs.Empty, EndAsyncEvent, null);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("OnHeartBeatTimerCallback Error Event: {0}\r\n\t{1}", ex.Message, ex.InnerException));
                }
            }
        }

        void EndAsyncEvent(IAsyncResult result)
        {
            try
            {
                EventHandler invokedMethod = ((AsyncResult)result).AsyncDelegate as EventHandler;

                invokedMethod.EndInvoke(result);
            }
            catch
            {
                Trace.WriteLine("An event listener went kaboom!");
            }
        }

        /// <summary>
        /// While connection is open, dequeue and send messages to the socket server
        /// </summary>
        void DequeueOutboundMessages()
        {
            while (!outboundQueue.IsAddingCompleted)
            {
                if (ReadyState == WebSocketState.Open)
                {
                    try
                    {
                        string msgString;

                        if (outboundQueue.TryTake(out msgString, 500))
                            _wsClient.Send(msgString);
                        else
                            MessageQueueEmptyEvent.Set();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("The outboundQueue is no longer open...");
                    }
                }
                else
                {
                    ConnectionOpenEvent.WaitOne(2000); // wait for connection event
                }
            }
        }

        /// <summary>
        /// <para>Client performs an initial HTTP POST to obtain a SessionId (sid) assigned to a client, followed
        ///  by the heartbeat timeout, connection closing timeout, and the list of supported transports.</para>
        /// <para>The tansport and sid are required as part of the ws: transport connection</para>
        /// </summary>
        /// <param name="uri">http://localhost:3000</param>
        /// <returns>Handshake object with sid value</returns>
        /// <example>DownloadString: 13052140081337757257:15:25:websocket,htmlfile,xhr-polling,jsonp-polling</example>
        void RequestHandshake(Uri uri)
        {
            string value = string.Empty;
            string errorText = string.Empty;

            using (WebClient client = new WebClient())
            {
                try
                {
                    if (HandShake.Headers.Count > 0)
                        client.Headers.Add(HandShake.Headers);

                    value = client.DownloadString(string.Format("{0}://{1}:{2}/socket.io/1/{3}", uri.Scheme, uri.Host, uri.Port, uri.Query)); // #5 tkiley: The uri.Query is available in socket.io's handshakeData object during authorization
                    // 13052140081337757257:15:25:websocket,htmlfile,xhr-polling,jsonp-polling
                    if (string.IsNullOrEmpty(value))
                        errorText = "Did not receive handshake from server";
                }
                catch (WebException webEx)
                {
                    Trace.WriteLine(string.Format("Handshake threw an exception...{0}", webEx.Message));
                    switch (webEx.Status)
                    {
                        case WebExceptionStatus.ConnectFailure:
                            errorText = string.Format("Unable to contact the server: {0}", webEx.Status);
                            break;
                        case WebExceptionStatus.NameResolutionFailure:
                            errorText = string.Format("Unable to resolve address: {0}", webEx.Status);
                            break;
                        case WebExceptionStatus.ProtocolError:
                            var resp = webEx.Response as HttpWebResponse;//((System.Net.HttpWebResponse)(webEx.Response))
                            if (resp != null)
                            {
                                switch (resp.StatusCode)
                                {
                                    case HttpStatusCode.Forbidden:
                                        errorText = "Socket.IO Handshake Authorization failed";
                                        break;
                                    default:
                                        errorText = string.Format("Handshake response status code: {0}", resp.StatusCode);
                                        break;
                                }
                            }
                            else
                                errorText = string.Format("Error getting handshake from Socket.IO host instance: {0}", webEx.Message);
                            break;
                        default:
                            errorText = string.Format("Handshake threw an exception...{0}", webEx.Message);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    errorText = string.Format("Error getting handshake from Socket.IO host instance: {0}", ex.Message);
                    //OnErrorEvent(this, new ErrorEventArgs(errMsg));
                }
            }

            if (string.IsNullOrEmpty(errorText))
                HandShake.UpdateFromSocketIOResponse(value);
            else
                HandShake.ErrorMessage = errorText;
        }














        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                Close();
                MessageQueueEmptyEvent.Dispose();
                ConnectionOpenEvent.Dispose();
            }
        }
    }
}
