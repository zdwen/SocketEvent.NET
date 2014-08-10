using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client;
using SocketIO.Client.Models.Entities.Messages.Event;
using TestForm.Entities.DTOs.Business;
using TestForm.Entities.DTOs.Operations;

namespace TestForm
{
    static class TestClient
    {
        //const string URL = "http://192.168.1.150:2900";
        const string URL = "http://127.0.0.1:3000";
        static Client _client;

        static TestClient()
        {
            _client = new Client(URL);
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

        public static void SendWzdEventNew()
        {
            DtoSocketIoEventBusiness<DtoStudent> eventInfo = new DtoSocketIoEventBusiness<DtoStudent>()
            {
                ClientID = "WzdClient",
                EventName = "WzdEvent",
                RequestID = Guid.NewGuid().ToString(),
                Data = new DtoStudent()
                {
                    Name = "Jack Bauer",
                    Age = 23,
                    Address = "LA America",
                },
            };

            _client.SendEvent(eventInfo);
        }

        public static void SendString()
        {

        }

        static void client_EventArrived(EventInfo<EventItemReceived> obj)
        {

        }

        //SocketEventClient client = SocketEventClientFactory.CreateInstance("WzdClient_Net", URL);
    }
}
