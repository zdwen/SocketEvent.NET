using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIO.Client.Models.Entities.Messages.Acknownledge
{
    public class Error
    {
        ///【闻祖东 2014-7-31-105227】2014/7/31 10:47:16-Ack	6:::1+[{"requestId":"64982e92-807f-4583-bf96-a42fe4e61bd6","status":","error":{"name":"ArgumentError","message":"Lack of argument [event].","stack":"ArgumentError: Lack of argument [event].\n    at exports.getError (G:\\DXListing\\Source\\SocketEvent\\libs\\exception\\exceptions.js:29:15)\n    at Object.MessageManager._validate (G:\\DXListing\\Source\\SocketEvent\\libs\\message\\message_manager.js:55:19)\n    at Object.MessageManager.subscribe (G:\\DXListing\\Source\\SocketEvent\\libs\\message\\message_manager.js:131:21)\n    at Object.<anonymous> (G:\\DXListing\\Source\\SocketEvent\\libs\\message\\message_manager.js:89:11)\n    at Socket.EventEmitter.emit [as $emit] (events.js:98:17)\n    at SocketNamespace.handlePacket (G:\\DXListing\\Source\\SocketEvent\\node_modules\\socket.io\\lib\\namespace.js:335:22)\n    at Manager.onClientMessage (G:\\DXListing\\Source\\SocketEvent\\node_modules\\socket.io\\lib\\manager.js:488:38)\n    at WebSocket.Transport.onMessage (G:\\DXListing\\Source\\SocketEvent\\node_modules\\socket.io\\lib\\transport.js:387:20)\n    at Parser.<anonymous> (G:\\DXListing\\Source\\SocketEvent\\node_modules\\socket.io\\lib\\transports\\websocket\\hybi-16.js:39:10)\n    at Parser.EventEmitter.emit (events.js:95:17)"}}]
        public string Name { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
    }
}
