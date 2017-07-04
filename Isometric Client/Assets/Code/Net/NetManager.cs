using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface;
using Isometric.Core;
using UnityEngine;

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

            BuildingsManager.Current.ShowArea(GetArea());

            GameUi.Current.ShowResources(GetResources());

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
                    GameUi.Current.ShowResources(GetResources());
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



        public MainBuildingInfo[,] GetArea()
        {
            return ((Dictionary<string, object>[,]) _requestProcessor.Request("get area")["buildings"])
                .TwoDimSelect(
                    b => new MainBuildingInfo
                    {
                        Name = (string) b["name"],
                        BuildingTime = (TimeSpan) b["real building time"],
                    });
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
        
        public float[] GetResources()
        {
            return (float[]) _requestProcessor.Request("get resources")["resources"];
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
            return (bool)_requestProcessor.Request(
                "add workers",
                new Dictionary<string, object>
                {
                    {"position", position},
                    {"delta", delta}
                })["success"];
        }

        public bool TryAddBuilders(Vector position, int delta)
        {
            return (bool)_requestProcessor.Request(
                "add builders",
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
                IsWorkerBuilding = (bool) response["is worker building"],
                Income = (float[]) response["income"],
            };
        }

        public ResearchInfo[] GetNearestResearches(out string current)
        {
            var response = _requestProcessor.Request("get nearest researches");

            current = (string) response["current"];
            return ((Dictionary<string, object>[]) response["researches"]).Select(
                r => new ResearchInfo
                {
                    Name = (string) r["name"],
                    NewBuildings = (string[]) r["new buildings"],
                }).ToArray();
        }

        public bool TryResearch(string researchName)
        {
            return (bool) _requestProcessor.Request(
                "research",
                new Dictionary<string, object>
                {
                    {"name", researchName},
                })["success"];
        }

        public void GetResearchPoints(out float currentPoints, out float requiredPoints, out float pointsPerMinute)
        {
            var response = _requestProcessor.Request("get research points");
             
            currentPoints   = (float) (double) response["current points"];
            requiredPoints  = (float) (double) response["required points"];
            pointsPerMinute = (float) (double) response["points per minute"];
        }



        public class BuildingInfo
        {
            public string OwnerName, Name;

            public int People, Builders, MaxBuilders, Workers, MaxWorkers;

            public bool Finished, IsIncomeBuilding, IsWorkerBuilding;

            public float[] Income;
        }

        public class MainBuildingInfo
        {
            public string Name;

            public TimeSpan BuildingTime;
        }

        public class ResearchInfo
        {
            public string Name;

            public string[] NewBuildings;
        }
    }
}
