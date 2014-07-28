using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketEvent;
using System.Threading;
using SocketEvent.Impl;

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
            client.BizArrived += new Func<SocketEventRequest, RequestResult>(client_BizArrived);
            client.BizSubscribed += new Action<SocketEventResponse>(client_BizSubscribed);
            client.Subscribe("PriceChanged");

            //client.Enqueue(eventName);
        }

        static void client_BizSubscribed(SocketEventResponse response)
        {
            SocketEventResponse serverResponse = response;
        }

        static RequestResult client_BizArrived(SocketEventRequest request)
        {
            SocketEventRequest serverRequest = request;
            return RequestResult.Success;
        }
    }
}
