using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SocketIOClient
{
    public class SocketIOHandshake
    {
        public string SessionID { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get { return !string.IsNullOrWhiteSpace(ErrorMessage); } }

        public void ResetConnection()
        {
            SessionID = ErrorMessage = string.Empty;
        }

        public void UpdateFromSocketIOResponse(string value)
        {
            ErrorMessage = string.Empty;
            string[] items = value.Split(new char[] { ':' });
            SessionID = items[0];
        }
    }
}
