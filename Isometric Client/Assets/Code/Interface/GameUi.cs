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
using Assets.Code.Interface.Buttons;
using Assets.Code.Interface.Modes;
using Assets.Code.Interface.Panels;
using Isometric.Dtos;

namespace Assets.Code.Interface
{
    public class GameUi : SingletonBehaviour<GameUi>
    {
        private bool _selectingTargetMode;
        private UiMode _mode;



        public Dictionary<UiMode, IModeUi> ModeUis { get; set; }

        public bool SelectingTargetMode
        {
            get { return _selectingTargetMode; }
            set
            {
                _selectingTargetMode = value;
                if (_selectingTargetMode)
                {
                    InformationPanel.Current.Text = "Выберите цель";
                }
            }
        }

        public Action<Vector> SelectingTargetAction;

        private TimeSpan _currentMessageTime;



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
                {UiMode.Management, new ManagementUi()},
            };
        }

        private void Update()
        {
            var deltaTime = TimeSpan.FromSeconds(Time.deltaTime);
            if (_currentMessageTime > TimeSpan.Zero)
            {
                _currentMessageTime -= deltaTime;

                if (_currentMessageTime <= TimeSpan.Zero)
                {
                    ShowPreviousData();
                }
            }

            ModeUis[Mode].Update(deltaTime);
        }



        public void ShowMessage(string message, TimeSpan time)
        {
            _currentMessageTime = time;
            InformationPanel.Current.Text = message;
        }

        public void ShowResources(ResourcesDto resources)
        {
            var i = 2;
            Ui.Current.ResourcesText.text =
                string.Format(
                    "Свободные люди: {0}/{1}\n"
                    + Names.ResourcesNames.Aggregate("", (sum, r) => sum + r + ": {" + i++ + "}\n"),
                    new object[]
                    {
                        resources.FreePeople,
                        resources.MaxPeople,
                    }.Concat(
                        resources.ResourcesArray.Select(r => (object) ((int) r).ToString())).ToArray());
        }
        
        public void SetMode(UiMode mode)
        {
            End();
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

        public bool SelectCell(Vector position)
        {
            if (SelectingTargetMode)
            {
                Debug.Log("Target selected");
                SelectingTargetAction(position);
                SelectingTargetMode = false;
                return false;
            }
            else
            {
                return ModeUis[Mode].SelectCell(position);
            }
        }

        public void HighlightCell(Vector position)
        {
            if (!_selectingTargetMode)
            {
                ModeUis[Mode].HighlightCell(position);
            }
        }

        public void ShowPreviousData()
        {
            if (!_selectingTargetMode)
            {
                ModeUis[Mode].ShowPreviousData();
            }
        }

        public void Clear()
        {
            InformationPanel.Current.Text = "";
            TableManager.Current.Clear();

            ModeUis[Mode].Clear();
        }

        public void End()
        {
            ModeUis[Mode].End();
        }
    }
}