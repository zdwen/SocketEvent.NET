using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client;
using System.Threading;
using SocketIO.Client.Models.Entities.Messages.Event;

namespace ConsoleTest
{
    class NewClientTest
    {
        const string URL = "http://192.168.1.112:2900";

        public static void Test()
        {
            NewClient client = new NewClient(URL);

            client.EventArrived += new Action<EventInfo<EventItemReceived>>(client_EventArrived);

            EventInfo<EventItemSent> eventInfo = new EventInfo<EventItemSent>()
            {
                Name = "subscribe",
                Args = new List<EventItemSent>()
                {
                    new EventItemSent() {
                        Event = "PriceChanged",
                        //Event="ChangeSalesState",
                        RequestId = Guid.NewGuid().ToString(),
                        SenderId="MerchantServiceClient",
                    },
                },
            };

            client.SendEvent(eventInfo);
        }

        static void client_EventArrived(EventInfo<EventItemReceived> obj)
        {
        }
    }
}
