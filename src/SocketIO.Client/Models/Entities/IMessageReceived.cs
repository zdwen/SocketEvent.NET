using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities
{
    interface IMessageReceived
    {
        string RawMessage { get; set; }
        void GenerateProperties();
    }
}
