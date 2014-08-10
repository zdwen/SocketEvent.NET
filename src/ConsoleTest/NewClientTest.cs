using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client;
using System.Threading;
using SocketIO.Client.Models.Entities.Messages.Event;

namespace ConsoleTest
{
    public static class NewClientTest
    {
        //const string URL = "http://192.168.1.112:2900";
        const string URL = "http://127.0.0.1:2900";

        public static void SubscribePriceChanged()
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
                        SenderId="WzdClient_Net",
                    },
                },
            };

            client.SendEvent(eventInfo);
        }

        public static void EnqueuePriceChanged()
        {

        }

        static void client_EventArrived(EventInfo<EventItemReceived> obj)
        {
        }
    }
}
