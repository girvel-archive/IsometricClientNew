using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Net;
using Assets.Code.Ui;
using Isometric.Core;
using UnityEngine;
using Resources = UnityEngine.Resources;

namespace Assets.Code
{
    public class NetManager : SingletonBehaviour<NetManager>
    {
        private RequestProcessor _requestProcessor;

        private Connection _connection;
        
        private readonly Encoding _encoding = Encoding.UTF8;



        public void Run(string login, string password)
        {
            _connection = new Connection(
                new IPEndPoint(
                    Dns.GetHostAddresses(Dns.GetHostName()).First(),
                    7999));
            _connection.Start();

            _requestProcessor = new RequestProcessor(_connection);

            _requestProcessor.Request(
                "login",
                new Dictionary<string, object>
                {
                        {"login", login},
                        {"password", password}
                },
                typeof(bool));

            BuildingsManager.Current.ShowArea(
                (Area)_requestProcessor.Request(
                    RequestType.GetArea,
                    new Dictionary<string, object>(),
                    typeof(Area))["area"]);

            UiManager.Current.ShowResources(
                (Isometric.Core.Resources)_requestProcessor.Request(
                    "get resources",
                    new Dictionary<string, object>(),
                    typeof(Isometric.Core.Resources))["resources"]);

            if ((bool)_requestProcessor.Request(
                "upgrade",
                new Dictionary<string, object>
                {
                        {"to", "House"},
                        {"position", new Vector(0, 0)}
                },
                typeof(bool))["success"])
            {
                BuildingsManager.Current.SetBuilding(new Vector2(0, 0), "House");
            }
        }

        public bool TryUpgrade(string name, Vector position)
        {
            return (bool) _requestProcessor.Request(
                "upgrade",
                new Dictionary<string, object>
                {
                    {"to", name},
                    {"position", position},
                }, 
                typeof(bool))["success"];
        }



        private void FixedUpdate()
        {
        }

        private void OnDestroy()
        {
            _connection.Dispose();
        }
    }
}
