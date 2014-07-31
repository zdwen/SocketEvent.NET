using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client;
using SocketIO.Client.Models.Entities.Messages.Event;
using ListingSocketEvent.Models.Entities;
using System.Threading.Tasks;
using ListingSocketEvent.Models.Enums;

namespace ListingSocketEvent
{
    public class SocketEventClient : IDisposable
    {
        public string Url { get; private set; }
        public string ListingSystemId { get; private set; }

        NewClient _socketIoClient;

        public event Action<EventInformation> BizArrived;

        public SocketEventClient(string url, string listingSystemId)
        {
            Url = url;
            ListingSystemId = listingSystemId;
            _socketIoClient = new NewClient(Url);
            _socketIoClient.EventArrived += EventArrivedHandler;
        }

        public void Subscribe(string bizName, dynamic args)
        {
            EventInfo<EventItemSent> eventInfo = new EventInfo<EventItemSent>()
            {
                Name = Operation.Subscribe.ToString().ToLower(),
                Args = new List<EventItemSent>()
                {
                    new EventItemSent() {
                        Event = bizName,
                        RequestId = Guid.NewGuid().ToString(),
                        SenderId = ListingSystemId,
                    },
                },
            };

            _socketIoClient.SendEvent(eventInfo);
        }

        void EventArrivedHandler(EventInfo<EventItemReceived> obj)
        {
            if (BizArrived != null && obj != null && obj.Args != null)
                foreach (EventItemReceived item in obj.Args)
                    Task.Factory.StartNew(() => BizArrived.Invoke(new EventInformation() { Name = item.Event, Args = item.Args, }));
        }

        public void Dispose()
        {
            _socketIoClient.Dispose();
        }
    }
}
