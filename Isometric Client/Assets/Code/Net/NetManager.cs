using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface;
using Isometric.Core.Vectors;
using Isometric.Dtos;
using UnityEngine;

namespace Assets.Code.Net
{
    public class NetManager : SingletonBehaviour<NetManager>
    {
        private RequestProcessor _requestProcessor;

        private NewsProcessor _newsProcessor;

        private Connection _connection;

        public string LastLogin, LastPassword;

        private IPAddress _lastIpAddress;

        private Vector _mainAreaPosition;

        public bool Runned { get; private set; }



        public void Run(string login, string password, IPAddress ip)
        {
            LastLogin = login;
            LastPassword = password;
            _lastIpAddress = ip;

            _connection = new Connection(new IPEndPoint(ip, 7999));

            if (!_connection.TryStart())
            {
                throw new ConnectionToServerException();
            }

            _requestProcessor = new RequestProcessor(_connection);

            if (!TryLogin(login, password))
            {
                throw new LoginException();
            }

            BuildingsManager.Current.ShowArea(GetArea());

            Runned = true;

            #region News

            _newsProcessor = new NewsProcessor(new Dictionary<string, Action<NewsDto>>
            {
                {
                    "army position changed", news =>
                    {
                        var oldPosition = (Vector) news.Info["old position"];

                        if (!GetArmiesInfo(oldPosition).Any())
                        {
                            BuildingsManager.Current.RemoveArmy(oldPosition);
                        }

                        BuildingsManager.Current.SetArmy((Vector) news.Info["new position"]);
                    }
                },
                {
                    "army is appeared", news =>
                    {
                        BuildingsManager.Current.SetArmy((Vector) news.Info["position"]);
                    }
                },
                {
                    "hunger is changed", news =>
                    {
                        var indicator = BuildingsManager.Current[(Vector) news.Info["position"]].Indicator;
                        
                        indicator.Hunger = (bool) news.Info["hunger"];
                    }
                },
            });

            #endregion
        }

        public void Stop()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }

        private void Update()
        {
            if (Runned)
            {
                _refreshDelay -= Time.deltaTime;

                if (_refreshDelay < 0)
                {
                    _refreshDelay += ResourcesRequestDelayDefault;
                    GameUi.Current.ShowResources(GetResources());
                    foreach (var news in GetNews())
                    {
                        _newsProcessor.Process(news);
                    }
                }

                if (!_connection.Connected)
                {
                    Runned = false;
                    ActionProcessor.Current.AddActionToQueue(() =>
                    {
                        Stop();
                        try
                        {
                            Run(LastLogin, LastPassword, _lastIpAddress);
                            Runned = true;
                            return true;
                        }
                        catch (LoginException)
                        {
                            return false;
                        }
                        catch (ConnectionToServerException)
                        {
                            return false;
                        }
                    }, 
                    TimeSpan.FromSeconds(1));
                }
            }
        }

        private const float ResourcesRequestDelayDefault = 1;
        private float _refreshDelay = ResourcesRequestDelayDefault;

        private void OnDestroy()
        {
            Stop();
        }



        #region Requests

        #region Area, login, resources

        public bool TryLogin(string login, string password)
        {
            return (bool) _requestProcessor.Request(
                "login",
                new Dictionary<string, object>
                {
                    {"login", login},
                    {"password", password}
                })["success"];
        }

        public Vector GetMainAreaPosition()
        {
            return (Vector)_requestProcessor.Request("get main area position")["position"];
        }

        public BuildingAreaDto[,] GetArea()
        {
            return (BuildingAreaDto[,])
                _requestProcessor.Request(
                    "get area",
                    new Dictionary<string, object>
                    {
                        {"position", _mainAreaPosition = GetMainAreaPosition()}
                    })["buildings"];
        }

        public float[] GetResources()
        {
            return (float[])_requestProcessor.Request("get resources")["resources"];
        }

        #endregion

        #region Buildings

        public bool TryUpgrade(string upgradeName, Vector position, out TimeSpan time)
        {
            var response = _requestProcessor.Request(
                "upgrade",
                new Dictionary<string, object>
                {
                    {"to", upgradeName},
                    {"position", new AbsolutePosition(_mainAreaPosition, position)},
                });

            time = (TimeSpan) response["upgrade time"];
            return (bool)     response["success"];
        }

        public string[] GetUpgrades(Vector position)
        {
            return (string[]) _requestProcessor.Request(
                "get upgrades",
                new Dictionary<string, object>
                {
                    {"position", new AbsolutePosition(_mainAreaPosition, position)},
                })["upgrades names"];
        }

        public BuildingFullDto GetBuildingInfo(Vector position)
        {
            return (BuildingFullDto) _requestProcessor.Request(
                "get building information",
                new Dictionary<string, object>
                {
                    {"position", new AbsolutePosition(_mainAreaPosition, position)},
                })["data"];
        }

        #endregion
        
        #region People management

        public bool TryAddWorkers(Vector position, int delta)
        {
            return (bool)_requestProcessor.Request(
                "add workers",
                new Dictionary<string, object>
                {
                    {"position", new AbsolutePosition(_mainAreaPosition, position)},
                    {"delta", delta}
                })["success"];
        }

        public bool TryAddBuilders(Vector position, int delta)
        {
            return (bool)_requestProcessor.Request(
                "add builders",
                new Dictionary<string, object>
                {
                    {"position", new AbsolutePosition(_mainAreaPosition, position)},
                    {"delta", delta}
                })["success"];
        }

        #endregion

        #region Researches

        public ResearchDto[] GetNearestResearches(out string current)
        {
            var response = _requestProcessor.Request("get nearest researches");

            current = (string) response["current"];
            return (ResearchDto[]) response["researches"];
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

        #endregion

        #region Armies

        public ArmyDto[] GetArmiesInfo(Vector position)
        {
            return (ArmyDto[])
                _requestProcessor.Request(
                    "get armies info",
                    new Dictionary<string, object>
                    {
                        {"position", new AbsolutePosition(_mainAreaPosition, position)},
                    })["armies"];
        }

        public void MoveArmy(Vector from, Vector to, int armyIndex)
        {
            _requestProcessor.Request(
                "move army",
                new Dictionary<string, object>
                {
                    {"position", new AbsolutePosition(_mainAreaPosition, from)},
                    {"to", to},
                    {"army index", armyIndex},
                });
        }

        public bool TrainArmy(Vector position, out TimeSpan trainingTime)
        {
            var response = _requestProcessor.Request(
                "train army",
                new Dictionary<string, object>
                {
                    {"position", new AbsolutePosition(_mainAreaPosition, position)},
                });

            trainingTime = (TimeSpan) response["training time"];
            return (bool) response["success"];
        }

        public TimeSpan LootBuilding(Vector position, int armyIndex)
        {
            return (TimeSpan) _requestProcessor.Request(
                "loot building",
                new Dictionary<string, object>
                {
                    {"position", new AbsolutePosition(_mainAreaPosition, position)},
                    {"army index", armyIndex},
                })["loot time"];
        }

        #endregion

        #region News

        public NewsDto[] GetNews()
        {
            return (NewsDto[]) _requestProcessor.Request("get news")["news"];
        }

        #endregion

        #endregion
    }
}
