using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace SocketIOClient.Messages
{
    /// <summary>
    /// All Socket.IO messages have to be encoded before they're sent, and decoded when they're received.
    /// They all have the format of: [message type] ':' [message id ('+')] ':' [message endpoint] (':' [message data])
    /// </summary>
    public abstract class MessageSioc : IMessageSioc
    {
        static Regex reMessageType = new Regex("^[0-8]{1}:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex re = new Regex(@"\d:\d?:\w?:");
        public static char[] _SplitChars = new char[] { ':' };

        public string RawMessage { get; protected set; }

        /// <summary>
        /// The message type represents a single digit integer [0-8].
        /// </summary>
        public SocketIOMessageTypes MessageType { get; protected set; }

        /// <summary>
        /// The message id is an incremental integer, required for ACKs (can be ommitted). 
        /// If the message id is followed by a +, the ACK is not handled by socket.io, but by the user instead.
        /// </summary>
        public int? AckId { get; set; }

        /// <summary>
        /// Socket.IO has built-in support for multiple channels of communication (which we call "multiple sockets"). 
        /// Each socket is identified by an endpoint (can be omitted).
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// String value of the message
        /// </summary>
        public string MessageText { get; set; }

        JsonEncodedEventMessage _json;
        public JsonEncodedEventMessage Json
        {
            get
            {
                if (_json == null)
                {
                    _json = (!string.IsNullOrEmpty(MessageText) && MessageText.Contains("name") && MessageText.Contains("args"))
                        ? JsonEncodedEventMessage.Deserialize(MessageText)
                        : new JsonEncodedEventMessage();
                }

                return _json;
            }
            set { _json = value; }
        }

        /// <summary>
        /// String value of the Event
        /// </summary>
        public virtual string Event { get; set; }

        /// <summary>
        /// <para>Messages have to be encoded before they're sent. The structure of a message is as follows:</para>
        /// <para>[message type] ':' [message id ('+')] ':' [message endpoint] (':' [message data])</para>
        /// <para>All message payloads are sent as strings</para>
        /// </summary>
        public virtual string Encoded
        {
            get
            {
                int msgId = (int)MessageType;
                
                return AckId.HasValue
                    ? string.Format("{0}:{1}:{2}:{3}", msgId, AckId, Endpoint, MessageText)
                    : string.Format("{0}::{1}:{2}", msgId, Endpoint, MessageText);
            }
        }

        public MessageSioc()
        {
            MessageType = SocketIOMessageTypes.Message;
        }

        public MessageSioc(string rawMessage)
            : this()
        {
            RawMessage = rawMessage;

            string[] args = rawMessage.Split(_SplitChars, 4);
            if (args.Length == 4)
            {
                int id;
                if (int.TryParse(args[1], out id))
                    AckId = id;
                Endpoint = args[2];
                MessageText = args[3];
            }
        }
        
        public static IMessageSioc Factory(string rawMsg)
        {
            if (reMessageType.IsMatch(rawMsg))
            {
                char id = rawMsg.First();
                switch (id)
                {
                    case '0':
                        return MessageSiocDisconnect.Deserialize(rawMsg);
                    case '1':
                        return MessageSiocConnect.Deserialize(rawMsg);
                    case '2':
                        return new MessageSiocHeartbeat();
                    case '3':
                        return MessageSiocText.Deserialize(rawMsg);
                    case '4':
                        return MessageSiocJson.Deserialize(rawMsg);
                    case '5':
                        return MessageSiocEvent.Deserialize(rawMsg);
                    case '6':
                        return MessageSiocAck.Deserialize(rawMsg);
                    case '7':
                        return MessageSiocError.Deserialize(rawMsg);
                    case '8':
                        return new MessageSiocNoop();
                    default:
                        Trace.WriteLine(string.Format("Message.Factory undetermined message: {0}", rawMsg));
                        return new MessageSiocText();
                }
            }
            else
            {
                Trace.WriteLine(string.Format("Message.Factory did not find matching message type: {0}", rawMsg));
                return new MessageSiocNoop();
            }
        }
    }
}
