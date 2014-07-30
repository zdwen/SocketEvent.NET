using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Messages.Acknownledge
{
    public class AcknowledgeItem
    {
        public string RequestId { get; set; }
        public string Status { get; set; }
    }
}
