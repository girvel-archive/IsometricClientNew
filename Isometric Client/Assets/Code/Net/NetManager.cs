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

            var mainAreaPosition = GetMainAreaPosition();
            Debug.Log(mainAreaPosition);
            var area = GetArea(mainAreaPosition);

            BuildingsManager.Current.Initialize(GetWorldWidth(), area.GetLength(0));

            BuildingsManager.Current.ShowArea(mainAreaPosition, area);

            Camera.main.transform.position
                = (Vector3) IsometricController.IsometricPositionToNormal(
                    mainAreaPosition.ToVector2() * area.GetLength(0),
                    Vector2.one)
                  + new Vector3(0, 0, -10);

            Runned = true;

            #region News

            _newsProcessor = new NewsProcessor(new Dictionary<string, Action<NewsDto>>
            {
                {
                    "army position is changed", news =>
                    {
                        var oldPosition = (Vector) news.Info["old position"];
                        var newPosition = (Vector) news.Info["new position"];

                        if (!GetArmiesInfo(oldPosition).Any())
                        {
                            BuildingsManager.Current.RemoveArmy(oldPosition);
                        }

                        BuildingsManager.Current.SetArmy(newPosition);

                        if (new[] {newPosition, oldPosition}.Any(p => p == BuildingsManager.Current.SelectedBuilding.Position))
                        {
                            ActionProcessor.Current.AddActionToQueue(() => GameUi.Current.Refresh(), TimeSpan.FromSeconds(0.25));
                        }
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
                {
                    "building is destroyed", news =>
                    {
                        BuildingsManager.Current.SetBuilding((Vector) news.Info["position"], "Plain", "no owner");
                        GameUi.Current.Refresh();
                    }
                },
                {
                    "army task is finished", news =>
                    {
                        if (BuildingsManager.Current.SelectedBuilding.Position == (Vector) news.Info["position"])
                        {
                            GameUi.Current.Refresh();
                        }
                    }
                },
                {
                    "building loot starts", news =>
                    {
                        BuildingsManager.Current.SetTimer(
                            (Vector) news.Info["position"], 
                            (TimeSpan) news.Info["loot time"]);
                    }
                }
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

        #region Debug and administration

        public bool TryExecute(string command, out string output)
        {
            var response = _requestProcessor.Request(
                "execute command",
                new Dictionary<string, object>
                {
                    {"command", command},
                });

            output = (string) response["output"];
            return (bool) response["success"];
        }

        #endregion

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
            return (Vector) _requestProcessor.Request("get main area position")["position"];
        }

        public int GetWorldWidth()
        {
            return (int) (long) _requestProcessor.Request("get world width")["world width"];
        }

        public BuildingAreaDto[,] GetArea(Vector position)
        {
            return (BuildingAreaDto[,])
                _requestProcessor.Request(
                    "get area",
                    new Dictionary<string, object>
                    {
                        {"position", position}
                    })["buildings"];
        }

        public ResourcesDto GetResources()
        {
            return (ResourcesDto)_requestProcessor.Request("get resources")["resources"];
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
                    {"position", position},
                });

            time = (TimeSpan) response["upgrade time"];
            return (bool)     response["success"];
        }

        public UpgradeDto[] GetUpgrades(Vector position)
        {
            return (UpgradeDto[]) _requestProcessor.Request(
                "get upgrades",
                new Dictionary<string, object>
                {
                    {"position", position},
                })["upgrades"];
        }

        public BuildingFullDto GetBuildingInfo(Vector position)
        {
            return (BuildingFullDto) _requestProcessor.Request(
                "get building information",
                new Dictionary<string, object>
                {
                    {"position", position},
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

        public IncomeBuildingDto[] GetAllIncomeBuildings()
        {
            return (IncomeBuildingDto[]) _requestProcessor.Request("get all income buildings")["buildings"];
        }

        public int AddWorkersForPrototype(string prototypeName, int delta)
        {
            return (int) (long) _requestProcessor.Request(
                "add workers for player",
                new Dictionary<string, object>
                {
                    {"name", prototypeName},
                    {"delta", delta},
                })["added"];
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

        public void ClearArmyTasksQueue(Vector position, int armyIndex)
        {
            _requestProcessor.Request(
                "clear army tasks queue",
                new Dictionary<string, object>
                {
                    {"position", position},
                    {"index", armyIndex},
                });
        }

        public ArmyDto[] GetArmiesInfo(Vector position)
        {
            return (ArmyDto[])
                _requestProcessor.Request(
                    "get armies info",
                    new Dictionary<string, object>
                    {
                        {"position", position},
                    })["armies"];
        }

        public void MoveArmy(Vector from, Vector to, int armyIndex)
        {
            _requestProcessor.Request(
                "move army",
                new Dictionary<string, object>
                {
                    {"position", from},
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
                    {"position", position},
                });

            trainingTime = (TimeSpan) response["training time"];
            return (bool) response["success"];
        }

        public void LootBuilding(Vector position, int armyIndex, Vector to, int range)
        {
            _requestProcessor.Request(
                "loot area",
                new Dictionary<string, object>
                {
                    {"position", position},
                    {"army index", armyIndex},
                    {"to", to},
                    {"range", range},
                });
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
