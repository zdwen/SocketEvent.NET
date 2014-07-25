using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIOClient.Messages;

namespace SocketIOClient
{
	class EndPointClient : IEndPointClient
	{
		public IClient Client { get; private set; }
		public string EndPoint { get; private set; }

		public EndPointClient(IClient client, string endPoint)
		{
			ValidateNameSpace(endPoint);
			Client = client;
			EndPoint = endPoint;
		}

		void ValidateNameSpace(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("nameSpace", "Parameter cannot be null");
			if (name.Contains(':'))
				throw new ArgumentException("Parameter cannot contain ':' characters", "nameSpace");
		}
			
		public void On(string eventName, Action<IMessageSioc> action)
		{
			Client.On(eventName, EndPoint, action);
		}

		public void Emit(string eventName, dynamic payload, Action<dynamic> callBack = null)
		{
			Client.Emit(eventName, payload, EndPoint, callBack);
		}

		public void Send(IMessageSioc msg)
		{
			msg.Endpoint = EndPoint;
			Client.Send(msg);
		}
	}
}
