using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SocketIOClient
{
    public class SocketIOHandshake
    {
        public string SID { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get { return !string.IsNullOrWhiteSpace(ErrorMessage); } }

        public void ResetConnection()
        {
            SID = ErrorMessage = string.Empty;
        }

        public void UpdateFromSocketIOResponse(string value)
        {
            ErrorMessage = string.Empty;
            string[] items = value.Split(new char[] { ':' });
            SID = items[0];
        }
    }
}
