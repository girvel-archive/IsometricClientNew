using System;
using System.Linq;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Net;
using Isometric.Core.Vectors;
using Isometric.Dtos;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Code.Interface.Modes
{
    public class ManagementUi : IModeUi
    {
        public void Refresh()
        {
            var incomeBuildings = NetManager.Current.GetAllIncomeBuildings();
            
            Ui.Current.ManagementForm.SetActive(true);

            foreach (var child in Ui.Current.ManagementForm.GetComponentsInChildren<ManagementRow>())
            {
                Object.Destroy(child.gameObject);
            }

            if (!incomeBuildings.Any())
            {
                return;
            }

            incomeBuildings = incomeBuildings.Concat(
                new[]
                {
                    new IncomeBuildingDto
                    {
                        Name = "Всего",
                        LastIncome = incomeBuildings
                            .Aggregate(
                                new float[incomeBuildings.First().LastIncome.Length],
                                (sum, b) => sum.Added(b.LastIncome)),
                        MaxIncome = incomeBuildings
                            .Aggregate(
                                new float[incomeBuildings.First().MaxIncome.Length],
                                (sum, b) => sum.Added(b.MaxIncome)),
                        Workers = incomeBuildings.Sum(b => b.Workers),
                        MaxWorkers = incomeBuildings.Sum(b => b.MaxWorkers)
                    },
                }).ToArray();

            var i = 0;
            foreach (var dto in incomeBuildings)
            {
                var row = Object.Instantiate(
                    Prefabs.Current.ManagementRow,
                    Ui.Current.ManagementForm.transform).GetComponent<ManagementRow>();

                var rowTransform = row.GetComponent<RectTransform>();

                rowTransform.offsetMin = new Vector2(0, 0);
                rowTransform.offsetMax = new Vector2(0, 35);
                rowTransform.anchoredPosition = new Vector2(0, -35 * (i + 1));
                
                string name;
                if (Names.RealBuildingsNames.TryGetValue(dto.Name, out name))
                {
                    row.Name = name;
                    row.RealName = dto.Name;
                }
                else
                {
                    row.Name = dto.Name;
                    row.PlusButton.gameObject.SetActive(false);
                    row.MinusButton.gameObject.SetActive(false);
                }
                row.Name += " (" + dto.Count + ") [за минуту]";

                var lastIncomeStr = dto.LastIncome.ToResourcesString();
                row.Income = lastIncomeStr == "-" ? "Не производит; [" + dto.MaxIncome.ToResourcesString() + "]" : lastIncomeStr;
                row.WorkersNow = dto.Workers;
                row.WorkersMax = dto.MaxWorkers;

                i++;
            }
        }

        public void ShowPreviousData()
        {
            
        }

        public bool SelectCell(Vector position)
        {
            return false;
        }

        public void HighlightCell(Vector position)
        {
            
        }

        public void Update(TimeSpan deltaTime)
        {
            
        }

        public void Clear()
        {
            Ui.Current.ManagementForm.SetActive(false);
        }
    }
}