using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketEvent
{
    public class SocketEventClient
    {
        public string Url { get; set; }

        public SocketEventClient(string url)
        {
            Url = url;
        }


    }
}
