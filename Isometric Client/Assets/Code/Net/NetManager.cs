using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Ui;
using Isometric.Core;
using UnityEngine;
using Resources = Isometric.Core.Resources;

namespace Assets.Code.Net
{
    public class NetManager : SingletonBehaviour<NetManager>
    {
        private RequestProcessor _requestProcessor;

        private Connection _connection;
        
        private readonly Encoding _encoding = Encoding.UTF8;

        public bool Runned { get; private set; }



        public void Run(string login, string password, IPAddress ip)
        {
            _connection = new Connection(new IPEndPoint(ip, 7999));
            _connection.Start();

            _requestProcessor = new RequestProcessor(_connection);

            _requestProcessor.Request(
                "login",
                new Dictionary<string, object>
                {
                    {"login", login},
                    {"password", password}
                });

            BuildingsManager.Current.ShowArea(
                (string[,]) _requestProcessor.Request(
                    RequestType.GetArea,
                    new Dictionary<string, object>())["buildings names"]);

            UiManager.Current.ShowResources(GetResources());

            Runned = true;
        }

        private void Update()
        {
            if (Runned)
            {
                _resourcesRequestDelay -= Time.deltaTime;

                if (_resourcesRequestDelay < 0)
                {
                    _resourcesRequestDelay += ResourcesRequestDelayDefault;
                    UiManager.Current.ShowResources(GetResources());
                }
            }
        }

        private const float ResourcesRequestDelayDefault = 1;
        private float _resourcesRequestDelay = ResourcesRequestDelayDefault;

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
        
        public Resources GetResources()
        {
            return (Resources) _requestProcessor.Request(
                "get resources",
                new Dictionary<string, object>())["resources"];
        }

        public string[] GetUpgrades(Vector position)
        {
            return (string[]) _requestProcessor.Request(
                "get upgrades",
                new Dictionary<string, object>
                {
                    {"position", position}
                })["upgrades names"];
        }

        public bool TryAddWorkers(Vector position, int delta)
        {
            return (bool) _requestProcessor.Request(
                "add workers",
                new Dictionary<string, object>
                {
                    {"position", position},
                    {"delta", delta}
                })["success"];
        }

        public int GetMaxWorkers(Vector position)
        {
            return (int) (long) _requestProcessor.Request(
                "get max workers",
                new Dictionary<string, object>
                {
                    {"position", position},
                })["max workers"];
        }

        public BuildingInfo GetBuildingInfo(Vector position)
        {
            var response = _requestProcessor.Request(
                "get building information",
                new Dictionary<string, object>
                {
                    {"position", position},
                });

            return new BuildingInfo
            {
                OwnerName = (string) response["owner name"],
                Name = (string) response["name"],
                People = (int) (long) response["people"],
                Builders = (int) (long) response["builders"],
                MaxBuilders = (int) (long) response["max builders"],
                Workers = (int) (long) response["workers"],
                MaxWorkers = (int) (long) response["max workers"],
                Finished = (bool) response["finished"],
                IsIncomeBuilding = (bool) response["is income building"],
                Income = (Resources) response["income"],
            };
        }



        public class BuildingInfo
        {
            public string OwnerName, Name;

            public int People, Builders, MaxBuilders, Workers, MaxWorkers;

            public bool Finished, IsIncomeBuilding;

            public Resources Income;
        }
    }
}
