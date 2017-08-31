using System;
using System.Linq;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface.Panels;
using Assets.Code.Interface.Table;
using Assets.Code.Net;
using Isometric.Core.Vectors;
using Isometric.Dtos;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interface.Modes
{
    public class BuildingUi : IModeUi
    {
        private BuildingFullDto _lastBuildingDto;

        private UpgradeDto[] _lastUpgrades;

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

        public bool SelectCell(Vector position)
        {
            _lastUpgrades = NetManager.Current.GetUpgrades(position);
            _lastBuildingDto = NetManager.Current.GetBuildingInfo(position);
            _lastPosition = position;

            GameUi.Current.Clear();

            TableManager.Current.Clear();
            var i = 0;
            foreach (var upgrade in _lastUpgrades)
            {
                TableManager.Current.SetButton(
                    i % 5, i / 5, Sprites.Current.GetIcon(upgrade.Name) ?? Sprites.Current.UpgradeIcon,
                    b =>
                    {
                        GameUi.Current.Clear();
                        TimeSpan upgradeTime;
                        if (NetManager.Current.TryUpgrade(upgrade.Name, _lastPosition, out upgradeTime))
                        {
                            BuildingsManager.Current.SetUpgrade(_lastPosition, upgrade.Name,
                                NetManager.Current.LastLogin, upgradeTime.Multiple(1 / Settings.Current.GameSpeed));

                            ActionProcessor.Current.AddActionToQueue(
                                () => GameUi.Current.Refresh(),
                                TimeSpan.FromSeconds(0.5));
                        }
                        else
                        {
                            GameUi.Current.ShowMessage("Улучшение невозможно", TimeSpan.FromSeconds(4));
                        }

                    },
                    string.Format(
                        "Улучшить до {0} ({1})"
                        + "\nСтоимость: {2}",
                        Names.RealBuildingsNames[upgrade.Name],
                        upgrade.Time.ToTimerString(),
                        upgrade.Price.ToResourcesString())
                    + (upgrade.RequiredResearches.Any()
                        ? "\n\nТребуются исследования: " +
                            upgrade.RequiredResearches.Aggregate("", (sum, r) => sum + "\n - " + r)
                        : ""));

                TableManager.Current.Buttons[i].GetComponent<Button>().interactable 
                    = upgrade.Possibility == UpgradePossibility.Possible;
                i++;
            }

            if (_lastBuildingDto.OwnerName == NetManager.Current.LastLogin || _lastBuildingDto.Name == "Plain")
            {
                if (_lastBuildingDto.IsArmyBuilding && _lastBuildingDto.IsFinished)
                {
                    TableManager.Current.SetButton(
                        2, 0, Sprites.Current.UpgradeIcon,
                        b =>
                        {
                            TimeSpan trainingTime;
                            NetManager.Current.TrainArmy(_lastPosition, out trainingTime);

                            BuildingsManager.Current.SetTimer(_lastPosition, trainingTime.Multiple(1 / Settings.Current.GameSpeed));
                        },
                        "Тренировать армию");
                }
            }

            ShowPreviousData();

            return true;
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
            if (_lastBuildingDto != null)
            {
                InformationPanel.Current.ShowInformation(_lastBuildingDto);
            }
        }

        public void Clear()
        {
            
        }

        private TimeSpan _currentRequestDelay = TimeSpan.FromSeconds(1);
    }
}