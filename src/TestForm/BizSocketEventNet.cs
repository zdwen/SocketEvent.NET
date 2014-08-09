using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketEvent.Impl;
using SocketEvent;
using SocketEvent.Dto;

namespace TestForm
{
    class BizSocketEventNet
    {
        static SocketEventClient _client;

        static BizSocketEventNet()
        {
            _client = SocketEventClientFactory.CreateInstance(RU.ClientID, RU.Url);

            _client.BizArrived += new Func<SocketEventRequestDto, RequestResult>(client_BizArrived);
            _client.BizSubscribed += new Action<SocketEventResponseDto>(client_BizSubscribed);
        }

        public static void Subscribe()
        {
            _client.Subscribe("PriceChanged");
        }

        public static void EnqueuePriceChanged()
        {
            _client.Enqueue("PriceChanged", 1, 60, new { ListingSku = "5100703" });
        }

        public static void EnqueuePublishSalesState()
        {
            _client.Enqueue("PublishSalesState", 1, 60, new { ListingSku = "5100704" });
        }

        static void client_BizSubscribed(SocketEventResponseDto resp)
        {
            SocketEventResponseDto serverResponse = resp;
        }

        static RequestResult client_BizArrived(SocketEventRequestDto request)
        {
            SocketEventRequestDto serverRequest = request;
            return RequestResult.Success;
        }
    }
}
