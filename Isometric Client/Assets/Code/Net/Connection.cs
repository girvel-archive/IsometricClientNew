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

        public bool Connected
        {
            get { return _socket.Connected; }
        }

        private Socket _socket;

        private readonly Encoding _encoding = Encoding.UTF8;



        public Connection(IPEndPoint serverEndPoint)
        {
            ServerEndPoint = serverEndPoint;
        }



        public bool TryStart()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _socket.Connect(ServerEndPoint);
            }
            catch (SocketException)
            {
                return false;
            }

            return true;
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
