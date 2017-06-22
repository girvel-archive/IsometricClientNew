using System;
using System.Linq;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Net;
using Assets.Code.Ui.Table;
using Isometric.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui
{
    public class UiManager : SingletonBehaviour<UiManager>
    {
        public InputField
            IpInputField,
            LoginInputField,
            PasswordInputField;

        public GameObject 
            LoginForm, 
            GameUiForm; 

        public Text ResourcesText;



        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                ClearBuildingSelection();
            }
        }



        public void ShowResources(Isometric.Core.Resources resources)
        {
            ResourcesText.text =
                string.Format(
                    "Wood: {0}\nFood: {1}",
                    resources.GetResourcesArray().Select(r => (object) r.ToString()).ToArray());
        }


        public void RefreshSelectedBuilding()
        {
            SelectBuilding(BuildingsManager.Current.SelectedBuilding.Position);
        }

        public void SelectBuilding(Vector buildingPosition)
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
                    () =>
                    {
                        var p = BuildingsManager.Current.SelectedBuilding.Position;

                        TimeSpan upgradeTime;
                        if (NetManager.Current.TryUpgrade(upgrade, p, out upgradeTime))
                        {
                            Debug.Log(upgradeTime);
                            BuildingsManager.Current.SetUpgrade(p, upgrade, upgradeTime);
                        }
                    });
                i++;
            }

            var info = NetManager.Current.GetBuildingInfo(buildingPosition);
            if (info.Workers > 0 && info.Finished)
            {
                TableManager.Current.SetButton(
                    0, 0, Sprites.Current.IncrementWorkers,
                    () =>
                    {
                        NetManager.Current.TryAddWorkers(buildingPosition, 1);
                    });

                TableManager.Current.SetButton(
                    1, 0, Sprites.Current.DecrementWorkers,
                    () =>
                    {
                        NetManager.Current.TryAddWorkers(buildingPosition, -1);
                    });
            }

            ShowInformation(buildingPosition);
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
    }
}