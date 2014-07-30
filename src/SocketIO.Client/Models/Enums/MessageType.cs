using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Enums
{
    /// <summary>
    /// 【闻祖东 2014-7-30-111548】每个枚举后面的注释为保留的原来的。每一个枚举对应的int值实际上都是Message发送过程中的首个数字对应类型。
    /// </summary>
    public enum MessageType
    {
        Disconnect = 0, //Signals disconnection. If no endpoint is specified, disconnects the entire socket.
        Connect = 1,    // Only used for multiple sockets. Signals a connection to the endpoint. Once the server receives it, it's echoed back to the client.
        Heartbeat = 2,
        Message = 3, // A regular message
        JSONMessage = 4, // A JSON message
        Event = 5, // An event is like a JSON message, but has mandatory name and args fields.
        Ack = 6,  //An acknowledgment contains the message id as the message data. If a + sign follows the message id, it's treated as an event message packet.
        Error = 7, // Error
        Noop = 8 // No operation
    }
}
