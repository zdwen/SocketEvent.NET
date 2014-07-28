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
    public class SocketEventClient : IDisposable
    {
        public const string SUBSCRIBE = "subscribe";
        public const string UNSUBSCRIBE = "unsubscribe";
        public const string ENQUEUE = "enqueue";

        public string ClientId { get; set; }
        public string Url { get; set; }

        Client _socketIoClient;

        public SocketEventClient(string url)
            : this(Guid.NewGuid().ToString(), url) { }

        public SocketEventClient(string id, string url)
        {
            ClientId = id;
            Url = url;

            InitSocketIoClient();
        }

        

        public void Dispose()
        {
            _socketIoClient.Dispose();
        }




        /// <summary>
        /// 【闻祖东 2014-7-28-162022】注册业务到达时的处理方法。
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public event Func<SocketEventRequest, RequestResult> BizArrived;
        /// <summary>
        /// 【闻祖东 2014-7-28-162248】业务订阅完成后的事件。
        /// </summary>
        public event Action<SocketEventResponse> BizSubscribed;

        void SocketHandler_On(IMessageSioc msg)
        {
            SocketEventRequestDto dto = JsonConvert.DeserializeObject<SocketEventRequestDto>(msg.Json.Args[0].ToString());
            SocketEventRequest request = Mapper.Map<SocketEventRequestDto, SocketEventRequest>(dto);
            RequestResult result = BizArrived.Invoke(request);

            // Simulate a ack callback because SocketIO4Net doesn't provide one by default.
            object[] ackObj = new object[] {
                    new SocketEventResponseDto() {
                        RequestId = request.RequestId,
                        Status = result.ToString().ToUpper()
                    }
                };

            MessageSiocAck ack = new MessageSiocAck()
            {
                AckId = msg.AckId,
                MessageText = JsonConvert.SerializeObject(ackObj),
            };

            ///TODO【闻祖东 2014-7-28-162131】这个地方即使是不Send也并不影响业务的完整性？？？
            _socketIoClient.Send(ack);
        }

        void SocketHandler_Emit(dynamic data)
        {
            JsonEncodedEventMessage json = data as JsonEncodedEventMessage;
            SocketEventResponseDto result = JsonConvert.DeserializeObject<SocketEventResponseDto>(json.Args[0]);
            SocketEventResponse response = Mapper.Map<SocketEventResponseDto, SocketEventResponse>(result);

            BizSubscribed.Invoke(response);
        }



        public void Subscribe(string eventName)
        {
            _socketIoClient.On(eventName, SocketHandler_On);

            SubscribeDto subscribeDto = new SubscribeDto()
            {
                Event = eventName,
                SenderId = ClientId
            };

            _socketIoClient.Emit(SUBSCRIBE, subscribeDto, string.Empty, SocketHandler_Emit);
        }

        /// <summary>
        /// TODO【闻祖东 2014-7-28-173407】这个地方，需要实现异步。
        /// </summary>
        public void Enqueue(string eventName, int tryTimes = 1, int timeout = 60, dynamic args = null)
        {
            EnqueueDto dto = new EnqueueDto()
            {
                Event = eventName,
                SenderId = ClientId,
                TryTimes = tryTimes == 0 ? 1 : tryTimes,
                Timeout = timeout,
                Args = args
            };

            _socketIoClient.Emit(ENQUEUE, dto, string.Empty, SocketHandler_Emit);
        }

        /// <summary>
        /// TODO【闻祖东 2014-7-28-165725】因为_dicEvents的数据的构造已经被破坏，这个ReSubscribe的业务需要重新实现。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete("【闻祖东 2014-7-28-165811】当前尚未重构，请勿使用该方法。")]
        void ReSubscribe(object sender, EventArgs e)
        {
            //foreach (KeyValuePair<string, dynamic> kvp in _dicEvents)
            //    Subscribe(kvp.Key);
        }

        void InitSocketIoClient()
        {
            _socketIoClient = new Client(Url)
            {
                RetryConnectionAttempts = int.MaxValue,
            };

            _socketIoClient.ConnectionReconnect += ReSubscribe;
            _socketIoClient.Connect();
        }
    }
}
