using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Message.Client
{
    public class MsgSiocEvent
    {
        public string Name { get; set; }
        public dynamic Args { get; set; }
    }
}
