using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Assets.Code.Net
{
    public class Connection : IDisposable
    {
        public IPEndPoint ServerEndPoint { get; set; }

        private Socket _socket;

        private readonly Encoding _encoding = Encoding.UTF8;



        public Connection(IPEndPoint serverEndPoint)
        {
            ServerEndPoint = serverEndPoint;
        }



        public void Start()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(ServerEndPoint);
        }
        
        public string Request(string request)
        {
            _socket.Send(_encoding.GetBytes(request));
            
            var currentStringBuilder = new StringBuilder();
            var receivedData = new byte[4096];

            do
            {
                var bytes = _socket.Receive(receivedData);

                currentStringBuilder.Append(_encoding.GetString(receivedData, 0, bytes));
            }
            while (_socket.Available > 0);

            Debug.Log(currentStringBuilder);

            return currentStringBuilder.ToString(); 
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                _socket.Close();
            }
        }
    }
}
