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
            Client client = new Client(URL);


            Thread.Sleep(5 * 1000);
            EventInfo eventInfo = new EventInfo()
            {
                Name = "PriceChanged",
                Args = new List<dynamic>()
                {
                    new {
                        Event = "PriceChanged",
                        RequestId = "64982e92-807f-4583-bf96-a42fe4e61bd6"
                    },
                },
            };

            client.SendEvent(eventInfo);
        }
    }
}
