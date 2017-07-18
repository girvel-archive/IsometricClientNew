using System;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Interface.Panels;
using Assets.Code.Interface.Table;
using Assets.Code.Net;
using Isometric.Core.Vectors;
using Isometric.Dtos;
using UnityEngine;

namespace Assets.Code.Interface.Modes
{
    public class BuildingUi : IModeUi
    {
        private BuildingFullDto _lastBuildingDto;

        private string[] _lastUpgrades;

        private Vector _lastPosition;



        public void Refresh()
        {
            if (BuildingsManager.Current.SelectedBuilding == null)
            {
                GameUi.Current.Clear();
            }
            else
            {
                SelectCell(BuildingsManager.Current.SelectedBuilding.Position);
            }
        }

        public void SelectCell(Vector position)
        {
            _lastUpgrades = NetManager.Current.GetUpgrades(position);
            _lastBuildingDto = NetManager.Current.GetBuildingInfo(position);
            _lastPosition = position;

            ShowPreviousData();
        }

        public void HighlightCell(Vector position)
        {
            InformationPanel.Current.ShowInformation(
                NetManager.Current.GetBuildingInfo(position));
        }

        public void Update(TimeSpan deltaTime)
        {
            _currentRequestDelay -= deltaTime;

            if (_currentRequestDelay <= TimeSpan.Zero 
                && BuildingsManager.Current.SelectedBuilding != null)
            {
                if (_lastBuildingDto != null
                    && _lastBuildingDto.IsFinished
                    && _lastBuildingDto.IsArmyBuilding)
                {
                    InformationPanel.Current.ShowInformation(
                        NetManager.Current.GetBuildingInfo(
                            BuildingsManager.Current.SelectedBuilding.Position));
                }

                _currentRequestDelay += TimeSpan.FromSeconds(1);
            }
        }

        public void ShowPreviousData()
        {
            GameUi.Current.Clear();

            if (_lastBuildingDto == null)
            {
                return;
            }

            TableManager.Current.Clear();
            var i = 5;
            foreach (var upgrade in _lastUpgrades)
            {
                TableManager.Current.SetButton(
                    i % 5, i / 5, Sprites.Current.UpgradeIcon,
                    b =>
                    {
                        GameUi.Current.Clear();
                        TimeSpan upgradeTime;
                        if (NetManager.Current.TryUpgrade(upgrade, _lastPosition, out upgradeTime))
                        {
                            BuildingsManager.Current.SetUpgrade(_lastPosition, upgrade, NetManager.Current.LastLogin, upgradeTime);
                        }

                        ActionProcessor.Current.AddActionToQueue(
                            () => GameUi.Current.Refresh(),
                            TimeSpan.FromSeconds(0.5));
                    },
                    "Улучшить до " + upgrade);
                i++;
            }

            if (_lastBuildingDto.IsArmyBuilding && _lastBuildingDto.IsFinished)
            {
                TableManager.Current.SetButton(
                    2, 0, Sprites.Current.UpgradeIcon,
                    b =>
                    {
                        TimeSpan trainingTime;
                        NetManager.Current.TrainArmy(_lastPosition, out trainingTime);

                        BuildingsManager.Current.SetTimer(_lastPosition, trainingTime);
                    },
                    "Тренировать армию");
            }

            if ((!_lastBuildingDto.IsFinished
                && _lastBuildingDto.MaxBuilders > 0)
                ||
                (_lastBuildingDto.IsFinished
                && _lastBuildingDto.IsWorkerBuilding
                && _lastBuildingDto.MaxWorkers > 0))
            {
                TableManager.Current.SetButton(
                    0, 0, Sprites.Current.IncrementWorkers,
                    _lastBuildingDto.IsFinished
                        ? new Action<TableButton>(b => NetManager.Current.TryAddWorkers(_lastPosition, 1))
                        : (b =>
                        {
                            if (NetManager.Current.TryAddBuilders(_lastPosition, 1))
                            {
                                var timer =
                                    BuildingsManager.Current.Buildings[_lastPosition.X, _lastPosition.Y].Indicator.Manager
                                        as Timer;

                                if (timer != null)
                                {
                                    if (timer.Infinite)
                                    {
                                        timer.Infinite = false;
                                    }
                                    else
                                    {
                                        timer.Value = timer.Value.Multiple(1 - 1.0f / (_lastBuildingDto.Builders + 1));
                                    }
                                }
                            }
                        }),
                    "Добавить одного " + (_lastBuildingDto.IsFinished ? "рабочего" : "строителя"));

                TableManager.Current.SetButton(
                    1, 0, Sprites.Current.DecrementWorkers,
                    _lastBuildingDto.IsFinished
                        ? new Action<TableButton>(b => NetManager.Current.TryAddWorkers(_lastPosition, -1))
                        : (b =>
                        {
                            if (NetManager.Current.TryAddBuilders(_lastPosition, -1))
                            {
                                var timer =
                                    BuildingsManager.Current.Buildings[_lastPosition.X, _lastPosition.Y].Indicator.Manager
                                        as Timer;

                                if (timer != null)
                                {
                                    if (_lastBuildingDto.Builders <= 1)
                                    {
                                        timer.Infinite = true;
                                    }
                                    else
                                    {
                                        timer.Value = timer.Value.Multiple(1 + 1.0f / (_lastBuildingDto.Builders - 1));
                                    }
                                }
                            }
                        }),
                    "Удалить одного " + (_lastBuildingDto.IsFinished ? "рабочего" : "строителя"));
            }

            InformationPanel.Current.ShowInformation(_lastBuildingDto);
        }

        public void Clear()
        {
            
        }

        private TimeSpan _currentRequestDelay = TimeSpan.FromSeconds(1);
    }
}