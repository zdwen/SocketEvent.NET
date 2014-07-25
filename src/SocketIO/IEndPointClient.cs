using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketIOClient.Messages;

namespace SocketIOClient
{
    /// <summary>
    /// 【闻祖东 2014-7-25-183254】感觉这个东东才是基于SocketIO进行的业务上的Client的封装。
    /// </summary>
	public interface IEndPointClient
	{
        /// <summary>
        /// 【闻祖东 2014-7-25-182133】捕获一个事件？？？
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
		void On(string eventName, Action<IMessageSioc> action);
        /// <summary>
        /// 【闻祖东 2014-7-25-182151】发送一个消息？？？
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="payload"></param>
        /// <param name="callBack"></param>
		void Emit(string eventName, dynamic payload, Action<dynamic> callBack = null);
        /// <summary>
        /// 【闻祖东 2014-7-25-182213】发送一个消息？？？
        /// </summary>
        /// <param name="msg"></param>
		void Send(IMessageSioc msg);
	}
}
