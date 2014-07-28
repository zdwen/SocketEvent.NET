using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient
{
    /// <summary>
    /// 【闻祖东 2014-7-25-165526】由ErrorEventArgs重命名至EventArgsSiocError。因为与SuperSocket.ClientEngine.ErrorEventArgs同名。
    /// </summary>
    public class EventArgsSiocError : EventArgs
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public EventArgsSiocError(string message)
            : base()
        {
            Message = message;
        }

        public EventArgsSiocError(string message, Exception exception)
            : base()
        {
            Message = message;
            Exception = exception;
        }
    }
}
