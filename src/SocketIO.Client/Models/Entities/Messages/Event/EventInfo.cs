using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Messages.Event
{
    public class EventInfo
    {
        public string Name { get; set; }
        public List<dynamic> Args { get; set; }
    }
}
