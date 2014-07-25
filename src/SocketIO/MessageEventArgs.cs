using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIOClient.Messages;

namespace SocketIOClient
{
	public class MessageEventArgs : EventArgs
	{
		public IMessageSioc Message { get; private set; }

		public MessageEventArgs(IMessageSioc msg)
			: base()
		{
			Message = msg;
		}
	}
}
