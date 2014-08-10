using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client.Models.Entities.Message.Server;

namespace SocketIO.Client
{
    public class Agent4Server
    {
        MsgSiocEvent GenEventFromEventMsg(string jsonString)
        {
            return CU.JsonDeserialize<MsgSiocEvent>(jsonString);
        }


    }
}
