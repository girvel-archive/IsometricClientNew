using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Code.Building;
using Assets.Code.Common;
using Assets.Code.Interface.Table;
using Assets.Code.Net;
using Isometric.Core.Vectors;
using UnityEngine;
using UnityEngine.UI;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface.Modes;
using Assets.Code.Interface.Panels;

namespace Assets.Code.Interface
{
    public class GameUi : SingletonBehaviour<GameUi>
    {
        public Dictionary<UiMode, IModeUi> ModeUis { get; set; }

        public bool SelectingTargetMode { get; set; }

        public Action<Vector> SelectingTargetAction;



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
            ModeUis = new Dictionary<UiMode, IModeUi>
            {
                {UiMode.Building, new BuildingUi()},
                {UiMode.Research, new ResearchUi()},
                {UiMode.Army, new ArmyUi()},
            };
        }

        private void Update()
        {
            ModeUis[Mode].Update(TimeSpan.FromSeconds(Time.deltaTime));
        }



        public void ShowResources(float[] resources)
        {
            Ui.Current.ResourcesText.text =
                string.Format(
                    "Дерево: {0}\n" +
                    "Еда: {1}",
                    resources.Select(r => (object)((int)r).ToString()).ToArray());
        }
        
        public void SetMode(UiMode mode)
        {
            Mode = mode;
            Refresh();
        }

        public void SetSelectingTargetAction(Action<Vector> selectingTargetAction)
        {
            SelectingTargetMode = true;
            Debug.Log(SelectingTargetMode);
            SelectingTargetAction = selectingTargetAction;
        }



        public void Refresh()
        {
            ModeUis[Mode].Refresh();
        }

        public void SelectCell(Vector position)
        {
            if (SelectingTargetMode)
            {
                Debug.Log("Target selected");
                SelectingTargetAction(position);
                SelectingTargetMode = false;
            }
            else
            {
                ModeUis[Mode].SelectCell(position);
            }
        }

        public void HighlightCell(Vector position)
        {
            ModeUis[Mode].HighlightCell(position);
        }

        public void ReselectLastCell()
        {
            ModeUis[Mode].ShowPreviousData();
        }

        public void Clear()
        {
            InformationPanel.Current.Text = "";
            TableManager.Current.Clear();

            ModeUis[Mode].Clear();
        }
    }
}