using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIO.Client.Models.Enums;

namespace SocketIO.Client.Models.Entities
{
    /// <summary>
    /// 【闻祖东 2014-7-30-155500】不需要Send
    /// </summary>
    public class MessageSiocConnect : MessageSioc
    {
        //【闻祖东 2014-7-30-154032】固定的1::
        public override MessageType MessageType { get { return MessageType.Connect; } }
    }
}
