using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketEvent.Impl;
using AutoMapper;
using SocketEvent.Dto;

namespace SocketEvent
{
    public class SocketEventClientFactory
    {
        /// <summary>
        /// Connect to a socket event URL
        /// </summary>
        /// <param name="url">Server URL</param>
        /// <returns>SocketEventClient</returns>
        public static SocketEventClient CreateInstance(string url)
        {
            return new SocketEventClient(url);
        }

        /// <summary>
        /// Connect to a socket event URL. Name the client with provided ID.
        /// </summary>
        /// <param name="id">ID of this client</param>
        /// <param name="url">Server URL</param>
        /// <returns>SocketEventClient</returns>
        public static SocketEventClient CreateInstance(string id, string url)
        {
            return new SocketEventClient(id, url);
        }
    }
}
