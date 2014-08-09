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
        //const string URL = "http://192.168.1.112:2900";
        const string URL = "http://192.168.1.150:2900";

        static void Main(string[] args)
        {
            //NewClientTest.SubscribePriceChanged();
            SubscribeTest();
            Console.ReadLine();
        }

        public static void SubscribeTest()
        {
            SocketEventClient client = SocketEventClientFactory.CreateInstance("WzdClient_Net", URL);
            //SocketEventClient client = SocketEventClientFactory.CreateInstance(URL);
            client.BizArrived += new Func<SocketEventRequestDto, RequestResult>(client_BizArrived);
            client.BizSubscribed += new Action<SocketEventResponseDto>(client_BizSubscribed);

            client.Subscribe("PriceChanged");
            //client.Subscribe("PublishSalesState");
            client.Enqueue("PriceChanged", 1, 60, new { ListingSku = "5100703" });
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
