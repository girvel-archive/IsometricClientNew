using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Net;
using Isometric.Core;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Code
{
    public class NetManager : SingletonBehaviour<NetManager>
    {
        private RequestProcessor _requestProcessor;

        private Connection _connection;
        
        private readonly Encoding _encoding = Encoding.UTF8;



        public void Run(string login, string password)
        {
            using (_connection = new Connection(
                new IPEndPoint(
                    Dns.GetHostAddresses(Dns.GetHostName()).First(), 
                    7999)))
            {
                _connection.Start();

                _requestProcessor = new RequestProcessor(_connection);

                _requestProcessor.Request(
                    "login",
                    new Dictionary<string, object>
                    {
                        {"login", login},
                        {"password", password}
                    },
                    typeof (bool));

                var area = (Area) _requestProcessor.Request(
                    RequestType.GetArea, 
                    new Dictionary<string, object>(), 
                    typeof(Area))["area"];

                for (var x = 0; x < area.Buildings.GetLength(0); x++)
                {
                    for (var y = 0; y < area.Buildings.GetLength(1); y++)
                    {
                        ((GameObject)Instantiate(Resources.Load<Object>("Plain")))
                            .GetComponent<IsometricController>()
                            .IsometricPosition = new Vector2(x, y);

                        switch (area.Buildings[x, y].BuildingTemplate.Name)
                        {
                            case "Forest":
                                ((GameObject)Instantiate(Resources.Load<Object>("Forest")))
                                    .GetComponent<IsometricController>()
                                    .IsometricPosition = new Vector2(x, y);
                                break;

                            case "House":
                                ((GameObject)Instantiate(Resources.Load<Object>("House - wood underground")))
                                    .GetComponent<IsometricController>()
                                    .IsometricPosition = new Vector2(x, y);
                                break;
                        }
                    }
                }
            }
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
