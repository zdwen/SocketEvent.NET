using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Message.Server
{
    public class MsgSiocEvent
    {
        public string Name { get; set; }
        public List<dynamic> Args { get; set; }
    }
}
