using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketIOClient.Messages
{
    public class MessageSiocError : MessageSioc
    {
		public string Reason { get; set; }
		public string Advice { get; set; }

		public override string Event
		{
			get { return "error"; }
		}

		public MessageSiocError()
        {
            MessageType = SocketIOMessageTypes.Error;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rawMessage">'7::' [endpoint] ':' [reason] '+' [advice]</param>
		/// <returns>ErrorMessage</returns>
		public static MessageSiocError Deserialize(string rawMessage)
		{
			MessageSiocError msg = new MessageSiocError();
			string[] args = rawMessage.Split(':');
			if (args.Length == 4)
			{
				msg.Endpoint = args[2];
				msg.MessageText = args[3];
				string[] complex = args[3].Split(new char[] { '+' });
				if (complex.Length > 1)
				{
					msg.Advice = complex[1];
					msg.Reason = complex[0];
				}
			}

			return msg;
		}
    }
}
