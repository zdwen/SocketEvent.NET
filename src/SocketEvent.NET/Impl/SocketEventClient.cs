using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SocketIOClient;
using SocketEvent.Dto;
using Newtonsoft.Json;
using SocketIOClient.Messages;
using AutoMapper;
using System.Collections.Concurrent;
using System.Threading;

namespace SocketEvent.Impl
{
    class SocketEventClient : ISocketEventClient
    {
        public const string SUBSCRIBE = "subscribe";
        public const string UNSUBSCRIBE = "unsubscribe";
        public const string ENQUEUE = "enqueue";

        /// <summary>
        /// 【闻祖东 2014-7-25-110254】用于保存订阅事件及参数以便重新注册。
        /// </summary>
        ConcurrentDictionary<string, dynamic> _dicEvents;
        Client _socketIoClient;
        Semaphore _locker;

        public SocketEventClient(string url)
            : this(Guid.NewGuid().ToString(), url) { }

        public SocketEventClient(string id, string url)
        {
            ClientId = id;
            Url = url;
            _dicEvents = new ConcurrentDictionary<string, dynamic>();
            _locker = new Semaphore(1, 1);

            InitSocketIoClient();
        }

        public string ClientId { get; set; }

        public ClientState State { get; set; }

        public string Url { get; set; }

        public void Subscribe(string eventName, Func<ISocketEventRequest, RequestResult> eventCallback, Action<ISocketEventResponse> subscribeReadyCallback = null)
        {
            _dicEvents[eventName] = new
            {
                eventCallback = eventCallback,
                subscribeReadyCallback = subscribeReadyCallback
            };

            DoSubscribe(eventName, eventCallback, subscribeReadyCallback);
        }

        public void Unsubscribe(string eventName, Action<ISocketEventResponse> callback)
        {
            throw new NotImplementedException();
        }

        public void Enqueue(string eventName, int tryTimes = 1, int timeout = 60, dynamic args = null, Action<ISocketEventResponse> callback = null)
        {
            _locker.WaitOne();
            var dto = new EnqueueDto()
            {
                Event = eventName,
                SenderId = ClientId,
                TryTimes = tryTimes == 0 ? 1 : tryTimes,
                Timeout = timeout,
                Args = args
            };

            _socketIoClient.Emit(ENQUEUE, dto, string.Empty, (data) =>
                {
                    var json = data as JsonEncodedEventMessage;
                    var result = JsonConvert.DeserializeObject<SocketEventResponseDto>(json.Args[0]);
                    var response = Mapper.Map<SocketEventResponseDto, SocketEventResponse>(result);

                    if (callback != null)
                        callback(response);

                    _locker.Release();
                });
        }

        public void Enqueue(string eventName, Action<ISocketEventResponse> callback)
        {
            Enqueue(eventName, 0, 60, null, callback);
        }

        public void Dispose()
        {
            // enqueue should have been finished within 60s.
            _locker.WaitOne(600 * 1000);///TODO【闻祖东 2014-7-25-183918】到底是应该60s还是600s？ 按照原代码和注释之间是互相不一致的。
            _socketIoClient.Dispose();
            _locker.Dispose();
        }



        void DoSubscribe(string eventName, Func<ISocketEventRequest, RequestResult> eventCallback, Action<ISocketEventResponse> subscribeReadyCallback)
        {
            _socketIoClient.On(eventName, (msg) =>
            {
                SocketEventRequestDto dto = JsonConvert.DeserializeObject<SocketEventRequestDto>(msg.Json.Args[0].ToString());
                SocketEventRequest request = Mapper.Map<SocketEventRequestDto, SocketEventRequest>(dto);
                RequestResult result = eventCallback(request);

                // Simulate a ack callback because SocketIO4Net doesn't provide one by default.
                object[] ackObj = new object[] {
                    new SocketEventResponseDto() {
                        RequestId = request.RequestId,
                        Status = result.ToString().ToUpper()
                    }
                };

                //string msgText = JsonConvert.SerializeObject(new object[] {
                //    new SocketEventResponseDto() {
                //        RequestId = request.RequestId,
                //        Status = result.ToString().ToUpper()
                //    }
                //});

                MessageSiocAck ack = new MessageSiocAck()
                {
                    AckId = msg.AckId,
                    MessageText = JsonConvert.SerializeObject(ackObj),
                };

                _socketIoClient.Send(ack);
            });

            SubscribeDto subscribeDto = new SubscribeDto()
            {
                Event = eventName,
                SenderId = ClientId
            };

            _socketIoClient.Emit(SUBSCRIBE, subscribeDto, string.Empty, (data) =>
            {
                JsonEncodedEventMessage json = data as JsonEncodedEventMessage;
                SocketEventResponseDto result = JsonConvert.DeserializeObject<SocketEventResponseDto>(json.Args[0]);
                SocketEventResponse response = Mapper.Map<SocketEventResponseDto, SocketEventResponse>(result);

                if (subscribeReadyCallback != null)
                    subscribeReadyCallback(response);
            });
        }

        void RedoSubscription(object sender, EventArgs e)
        {
            foreach (var entry in _dicEvents)
                DoSubscribe(entry.Key, entry.Value.eventCallback, entry.Value.subscribeReadyCallback);
        }

        void InitSocketIoClient()
        {
            _socketIoClient = new Client(Url)
            {
                RetryConnectionAttempts = int.MaxValue,
            };

            _socketIoClient.ConnectionReconnect += RedoSubscription;
            _socketIoClient.Connect();
        }
    }
}
