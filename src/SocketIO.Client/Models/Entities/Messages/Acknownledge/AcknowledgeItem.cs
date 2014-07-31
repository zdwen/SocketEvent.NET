using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client.Models.Enums.Acknownledge;

namespace SocketIO.Client.Models.Entities.Messages.Acknownledge
{
    public class AcknowledgeItem
    {
        public string RequestId { get; set; }
        public Status Status { get; set; }
        public Error Error { get; set; }
    }
}
