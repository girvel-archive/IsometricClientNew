using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Net;
using Assets.Code.Ui;
using Isometric.Core;

namespace Assets.Code
{
    public class NetManager : SingletonBehaviour<NetManager>
    {
        private IsometricRequestProcessor _requestProcessor;

        private Connection _connection;
        
        private readonly Encoding _encoding = Encoding.UTF8;

        public bool Runned { get; private set; }



        public void Run(string login, string password)
        {
            _connection = new Connection(
                new IPEndPoint(
                    Dns.GetHostAddresses(Dns.GetHostName()).First(),
                    7999));
            _connection.Start();

            _requestProcessor = new IsometricRequestProcessor(_connection);

            _requestProcessor.Request(
                "login",
                new Dictionary<string, object>
                {
                        {"login", login},
                        {"password", password}
                });

            BuildingsManager.Current.ShowArea(
                (Area)_requestProcessor.Request(
                    RequestType.GetArea,
                    new Dictionary<string, object>())["area"]);

            UiManager.Current.ShowResources(_requestProcessor.GetResources());

            Runned = true;
        }
        
        private void FixedUpdate()
        {
            if (Runned)
            {
                UiManager.Current.ShowResources(_requestProcessor.GetResources());
            }
        }

        private void OnDestroy()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }



        public bool TryUpgrade(string upgradeName, Vector position, out TimeSpan time)
        {
            var response = _requestProcessor.Request(
                "upgrade",
                new Dictionary<string, object>
                {
                    {"to", upgradeName},
                    {"position", position},
                });

            time = (TimeSpan) response["upgrade time"];
            return (bool)     response["success"];
        }
    }
}
