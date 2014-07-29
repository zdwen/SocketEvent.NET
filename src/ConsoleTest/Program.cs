using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketEvent;
using System.Threading;
using SocketEvent.Impl;
using SocketEvent.Dto;

namespace ConsoleTest
{
    class Program
    {
        const string URL = "http://192.168.1.112:2900";

        static void Main(string[] args)
        {
            //ISocketEventClient client = SocketEventClientFactory.CreateInstance(URL);
            SubscribeTest();
            Console.ReadLine();
        }

        public static void SubscribeTest()
        {
            SocketEventClient client = SocketEventClientFactory.CreateInstance("MerchantServiceClient", URL);
            client.BizArrived += new Func<SocketEventRequestDto, RequestResult>(client_BizArrived);
            client.BizSubscribed += new Action<SocketEventResponseDto>(client_BizSubscribed);

            client.Subscribe("PriceChanged");
            client.Subscribe("PublishSalesState");
            //client.Enqueue(eventName);
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
