using System;
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

namespace Assets.Code.Interface
{
    public class GameUi : SingletonBehaviour<GameUi>
    {
        public UiMode Mode;


        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                ClearBuildingSelection();
            }
        }



        public void ShowResources(Isometric.Core.Resources resources)
        {
            Ui.Current.ResourcesText.text =
                string.Format(
                    "Wood: {0}\nFood: {1}",
                    resources.GetResourcesArray().Select(r => (object)((int)r).ToString()).ToArray());
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
            TableManager.Current.Clear();

            var researches = NetManager.Current.GetNearestResearches();
            for (var i = 0; i < researches.Length; i++)
            {
                var research = researches[i];

                TableManager.Current.SetButton(
                    i % 5,
                    i / 5,
                    Sprites.Current.GetResearchSprite(research),
                    b =>
                    {
                        if (NetManager.Current.TryResearch(research))
                        {
                            b.HotkeyText.gameObject.SetActive(false);
                            b.InformationText.gameObject.SetActive(true);
                            b.InformationText.text = "0%";
                            b.GetComponent<Button>().interactable = false;

                            ActionProcessor.Current.AddActionToQueue(() =>
                            {
                                if (Current.Mode != UiMode.Research) return true;

                                float current, required;
                                NetManager.Current.GetResearchPoints(out current, out required);

                                if (current >= required)
                                {
                                    RefreshTable();
                                    return true;
                                }

                                b.InformationText.text = (int) (current / required * 100) + "%";

                                return false;
                            }, 
                            TimeSpan.FromSeconds(1));
                        }
                    },
                    "Research " + research);
            }
        }

        public void RefreshSelectedBuilding()
        {
            SelectBuilding(BuildingsManager.Current.SelectedBuilding.Position);
        }

        public void SelectBuilding(Vector buildingPosition)
        {
            ClearBuildingSelection();
            if (Mode == UiMode.Building)
            {
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
                    if ((!info.Finished && info.MaxBuilders > 0) || (info.Finished && info.IsIncomeBuilding && info.MaxWorkers > 0))
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
                                        timer.Value = timer.Value.Multiple(1 - 1.0f / (info.Builders + 1));
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

                                        Debug.Log(1 + 1.0f / (info.Builders - 1));
                                        timer.Value = timer.Value.Multiple(1 + 1.0f / (info.Builders - 1));
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