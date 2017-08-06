using System;
using System.Linq;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface.Panels;
using Assets.Code.Interface.Table;
using Assets.Code.Net;
using Isometric.Core.Vectors;
using Isometric.Dtos;
using UnityEngine.UI;

namespace Assets.Code.Interface.Modes
{
    public class ResearchUi : IModeUi
    {
        private BuildingFullDto _lastBuildingDto;

        private ResearchDto[] _lastResearchesDtos;

        private string _lastCurrentResearch;



        public void Refresh()
        {
            GameUi.Current.Clear();
            
            ShowResearches();
        }

        public bool SelectCell(Vector position)
        {
            HighlightCell(position);

            return true;
        }

        public void HighlightCell(Vector position)
        {
            _lastBuildingDto = NetManager.Current.GetBuildingInfo(position);
            ShowPreviousData();
        }

        public void Update(TimeSpan deltaTime)
        {
            
        }

        public void ShowPreviousData()
        {
            if (_lastBuildingDto == null)
            {
                return;
            }

            InformationPanel.Current.ShowInformation(_lastBuildingDto);
        }

        public void Clear()
        {
            ShowResearches();
        }



        private void ShowResearches()
        {
            Action<TableButton, ResearchDto> showResearchProgress = (b, research) =>
            {
                b.SetMode(false);
                b.InformationText.text = "";
                b.GetComponent<Button>().interactable = false;

                ActionProcessor.Current.AddActionToQueue(
                    () =>
                    {
                        if (GameUi.Current.Mode != UiMode.Research) return true;

                        float current, required, perMinute;
                        NetManager.Current.GetResearchPoints(out current, out required, out perMinute);

                        if (current >= required)
                        {
                            GameUi.Current.Refresh();
                            return true;
                        }

                        b.InformationText.text = (int)(current / required * 100) + "%";
                        b.Description =
                            "Research {0} ({1}/{2})\n\nPoints per minute: {3}\nNew buildings: {4}"
                                .FormatBy(
                                    research.Name,
                                    (int)current,
                                    (int)required,
                                    (int)perMinute,
                                    research.NewBuildings.Any()
                                        ? research.NewBuildings
                                            .Aggregate("", (sum, buildingName) => sum + ", " + buildingName)
                                            .Substring(2)
                                        : "");

                        return false;
                    },
                    TimeSpan.FromSeconds(1));
            };

            _lastResearchesDtos = NetManager.Current.GetNearestResearches(out _lastCurrentResearch);
            for (var i = 0; i < _lastResearchesDtos.Length; i++)
            {
                var research = _lastResearchesDtos[i];

                if (research.Name == _lastCurrentResearch)
                {
                    showResearchProgress(TableManager.Current.Buttons[i], research);
                }

                TableManager.Current.SetButton(
                    i % 5,
                    i / 5,
                    Sprites.Current.GetIcon(research.Name),
                    b =>
                    {
                        if (NetManager.Current.TryResearch(research.Name))
                        {
                            showResearchProgress(b, research);
                        }
                    },
                    "Исследование '{0}'\n\nОткрывает здания: {1}"
                        .FormatBy(
                            research.Name,
                            research.NewBuildings.Any()
                                ? research.NewBuildings
                                    .Aggregate("", (sum, buildingName) => sum + ", " + buildingName)
                                    .Substring(2)
                                : ""));
            }
        }
    }
}