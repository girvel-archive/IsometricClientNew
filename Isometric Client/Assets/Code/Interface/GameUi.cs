using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Interface.Table;
using Assets.Code.Net;
using Isometric.Core;
using Isometric.Core.Common;
using UnityEngine;
using UnityEngine.UI;
using Assets.Code.Common.Helpers;

namespace Assets.Code.Interface
{
    public class GameUi : SingletonBehaviour<GameUi>
    {
        public UiMode Mode
        {
            get { return _mode; }
            set
            {
                ModeButtons[(int) Mode].SetActiveFrame(false);
                _mode = value;
                ModeButtons[(int) Mode].SetActiveFrame(true);
            }
        }
        private UiMode _mode;

        public ModeButton[] ModeButtons;


        protected override void Start()
        {
            base.Start();

            Mode = UiMode.Building;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                ClearBuildingSelection();
            }
        }



        public void ShowResources(float[] resources)
        {
            Ui.Current.ResourcesText.text =
                string.Format(
                    "Wood: {0}\nFood: {1}",
                    resources.Select(r => (object)((int)r).ToString()).ToArray());
        }


        public void SetMode(UiMode mode)
        {
            ClearBuildingSelection();

            switch (mode)
            {
                case UiMode.Research:
                    RefreshResearches();
                    break;
            }
            Mode = mode;
        }

        public void RefreshResearches()
        {
            Action<TableButton, NetManager.ResearchInfo> showResearchProgress = (b, research) =>
            {
                b.SetMode(false);
                b.InformationText.text = "";
                b.GetComponent<Button>().interactable = false;

                ActionProcessor.Current.AddActionToQueue(
                    () =>
                    {
                        if (Current.Mode != UiMode.Research) return true;

                        float current, required, perMinute;
                        NetManager.Current.GetResearchPoints(out current, out required, out perMinute);

                        if (current >= required)
                        {
                            RefreshTable();
                            return true;
                        }

                        b.InformationText.text = (int) (current / required * 100) + "%";
                        b.Description =
                            "Research {0} ({1}/{2})\n\nPoints per minute: {3}\nNew buildings: {4}"
                                .FormatBy(
                                    research.Name,
                                    (int) current,
                                    (int) required,
                                    (int) perMinute,
                                    research.NewBuildings.Any()
                                        ? research.NewBuildings
                                            .Aggregate("", (sum, buildingName) => sum + ", " + buildingName)
                                            .Substring(2)
                                        : ""); // TODO optimize

                        return false;
                    },
                    TimeSpan.FromSeconds(1));
            };

            TableManager.Current.Clear();

            string currentResearch;
            var researches = NetManager.Current.GetNearestResearches(out currentResearch);
            for (var i = 0; i < researches.Length; i++)
            {
                var research = researches[i];

                if (research.Name == currentResearch)
                {
                    showResearchProgress(TableManager.Current.Buttons[i], research);
                }

                TableManager.Current.SetButton(
                    i % 5,
                    i / 5,
                    Sprites.Current.GetResearchSprite(research.Name),
                    b =>
                    {
                        if (NetManager.Current.TryResearch(research.Name))
                        {
                            showResearchProgress(b, research);
                        }
                    },
                    "Research {0}\n\nNew buildings: {1}"
                        .FormatBy(
                            research.Name,
                            research.NewBuildings.Any()
                                ? research.NewBuildings
                                    .Aggregate("", (sum, buildingName) => sum + ", " + buildingName)
                                    .Substring(2)
                                : "")); // TODO optimize);
            }
        }

        public void RefreshSelectedBuilding()
        {
            SelectBuilding(BuildingsManager.Current.SelectedBuilding.Position);
        }

        public void SelectBuilding(Vector buildingPosition)
        {
            if (Mode == UiMode.Building)
            {
                ClearBuildingSelection();

                BuildingsManager.Current.Buildings[buildingPosition.X, buildingPosition.Y].Holder
                    .GetComponent<SpriteRenderer>().sprite = Sprites.Current.SelectedPlain;

                BuildingsManager.Current.SelectedBuilding = BuildingsManager.Current.Buildings[buildingPosition.X, buildingPosition.Y];

                TableManager.Current.Clear();
                var i = 5;
                foreach (var upgrade in NetManager.Current.GetUpgrades(buildingPosition))
                {
                    TableManager.Current.SetButton(
                        i % 5, i / 5, Sprites.Current.UpgradeIcon,
                        b =>
                        {
                            TimeSpan upgradeTime;
                            if (NetManager.Current.TryUpgrade(upgrade, buildingPosition, out upgradeTime))
                            {
                                Debug.Log(upgradeTime);
                                BuildingsManager.Current.SetUpgrade(buildingPosition, upgrade, upgradeTime);
                            }
                        },
                        "Upgrade to " + upgrade);
                    i++;
                }

                ActionProcessor.Current.AddActionToQueue(() =>
                {
                    var info = NetManager.Current.GetBuildingInfo(buildingPosition);
                    if ((!info.Finished && info.MaxBuilders > 0) || (info.Finished && info.IsWorkerBuilding && info.MaxWorkers > 0))
                    {
                        TableManager.Current.SetButton(
                            0, 0, Sprites.Current.IncrementWorkers,
                            info.Finished
                                ? new Action<TableButton>(b => NetManager.Current.TryAddWorkers(buildingPosition, 1))
                                : (b =>
                                {
                                    if (NetManager.Current.TryAddBuilders(buildingPosition, 1))
                                    {
                                        var timer =
                                            BuildingsManager.Current.Buildings[buildingPosition.X, buildingPosition.Y]
                                                .Timer;
                                        if (timer.Infinite)
                                        {
                                            timer.Infinite = false;
                                        }
                                        else
                                        {
                                            timer.Value = timer.Value.Multiple(1 - 1.0f / (info.Builders + 1));
                                        }
                                    }
                                }),
                            "Adds one " + (info.Finished ? "worker" : "builder"));

                        TableManager.Current.SetButton(
                            1, 0, Sprites.Current.DecrementWorkers,
                            info.Finished
                                ? new Action<TableButton>(b => NetManager.Current.TryAddWorkers(buildingPosition, -1))
                                : (b =>
                                {
                                    if (NetManager.Current.TryAddBuilders(buildingPosition, -1))
                                    {
                                        var timer =
                                            BuildingsManager.Current.Buildings[buildingPosition.X, buildingPosition.Y]
                                                .Timer;
                                        if (info.Builders <= 1)
                                        {
                                            timer.Infinite = true;
                                        }
                                        else
                                        {
                                            timer.Value = timer.Value.Multiple(1 + 1.0f / (info.Builders - 1));
                                        }
                                    }
                                }),
                            "Removes one " + (info.Finished ? "worker" : "builder"));
                    }

                    InformationPanel.Current.ShowInformation(info);
                });
            }
        }

        public void ClearBuildingSelection()
        {
            if (BuildingsManager.Current.SelectedBuilding != null)
            {
                BuildingsManager.Current.SelectedBuilding.Holder.GetComponent<SpriteRenderer>().sprite =
                    Sprites.Current.UsualPlain;
            }

            BuildingsManager.Current.SelectedBuilding = null;

            InformationPanel.Current.Text = "";
            TableManager.Current.Clear();
        }

        public void ShowInformation(Vector buildingPosition)
        {
            InformationPanel.Current.ShowInformation(NetManager.Current.GetBuildingInfo(buildingPosition));
        }

        public void RefreshTable()
        {
            switch (Mode)
            {
                case UiMode.Building:
                    RefreshSelectedBuilding();
                    break;

                case UiMode.Research:
                    RefreshResearches();
                    break;
            }
        }
    }
}