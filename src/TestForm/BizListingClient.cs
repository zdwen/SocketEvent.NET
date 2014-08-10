using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketEvent;
using SocketIO.Client;
using SocketIO.Client.Models.Entities.Messages.Event;

namespace TestForm
{
    static class BizListingClient
    {
        //const string URL = "http://192.168.1.150:2900";
        const string URL = "http://127.0.0.1:3000";
        static NewClient _client;

        static BizListingClient()
        {
            _client = new NewClient(URL);
            _client.EventArrived += new Action<EventInfo<EventItemReceived>>(client_EventArrived);
        }

        public static void SendWzdEvent()
        {
            EventInfo<EventItemSent> eventInfo = new EventInfo<EventItemSent>()
            {
                Name = "WzdEvent",
                Args = new List<EventItemSent>()
                {
                    new EventItemSent(){
                     Event="WzdEventItem",
                      RequestId=Guid.NewGuid().ToString(),
                       SenderId="WzdClient"
                    },
                },
            };

            _client.SendEvent(eventInfo);
        }

        static void client_EventArrived(EventInfo<EventItemReceived> obj)
        {

        }

        //SocketEventClient client = SocketEventClientFactory.CreateInstance("WzdClient_Net", URL);
    }
}
