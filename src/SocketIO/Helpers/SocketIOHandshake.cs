using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SocketIOClient
{
    public class SocketIOHandshake
    {
        int _connTimeout { get; set; }
        List<string> _transports = new List<string>();

        public string SID { get; set; }
        public int HeartbeatTimeout { get; set; }
        public string ErrorMessage { get; set; }
        public NameValueCollection Headers { get; set; }

        public bool HasError { get { return !string.IsNullOrWhiteSpace(ErrorMessage); } }
        /// <summary>
        /// The HearbeatInterval will be approximately 20% faster than the Socket.IO service indicated was required
        /// </summary>
        public TimeSpan HeartbeatInterval { get { return new TimeSpan(0, 0, HeartbeatTimeout); } }




        public SocketIOHandshake()
        {
            Headers = new NameValueCollection();
        }

        public SocketIOHandshake(NameValueCollection headers)
        {
            Headers = headers == null
                ? new NameValueCollection()
                : headers;
        }

        public void ResetConnection()
        {
            SID = ErrorMessage = string.Empty;
        }

        public void UpdateFromSocketIOResponse(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                ErrorMessage = string.Empty;
                string[] items = value.Split(new char[] { ':' });
                if (items.Count() == 4)
                {
                    SID = items[0];

                    int hb = 0;
                    if (int.TryParse(items[1], out hb))
                    {
                        var pct = (int)(hb * .75);  // setup client time to occur 25% faster than needed
                        HeartbeatTimeout = pct;
                    }

                    int ct = 0;
                    if (int.TryParse(items[2], out ct))
                        _connTimeout = ct;

                    _transports.AddRange(items[3].Split(new char[] { ',' }));
                }
            }
        }
    }
}
