using System;
using WebSocket4Net;
using SocketIOClient.Messages;

namespace SocketIOClient
{
	/// <summary>
	/// C# Socket.IO client interface
	/// </summary>
	interface IClient
	{
		event EventHandler Opened;
		event EventHandler<MessageEventArgs> Message;
		event EventHandler Closed;
		event EventHandler<EventArgsSiocError> Error;

		SocketIOHandshake HandShake { get; }
		bool IsConnected { get; }
		WebSocketState ReadyState { get; }

		void Connect();
		IEndPointClient Connect(string endPoint);

		void Close();

		void On(string eventName, Action<IMessageSioc> action);
		void On(string eventName, string endPoint, Action<IMessageSioc> action);

		void Emit(string eventName, dynamic payload);
		void Emit(string eventName, dynamic payload, string endPoint = "", Action<dynamic> callBack = null);
		
		void Send(SocketIOClient.Messages.IMessageSioc msg);
		//void Send(string rawEncodedMessageText);
	}
}
