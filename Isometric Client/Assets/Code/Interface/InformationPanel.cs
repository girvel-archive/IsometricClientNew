using System.Linq;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Net;
using Isometric.Dtos;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interface
{
    public class InformationPanel : SingletonBehaviour<InformationPanel>
    {
        public string Text
        {
            get { return GetComponent<Text>().text; }
            set { GetComponent<Text>().text = value; }
        }

        public Image Background;

        private float _fixedMaxOffsetHeight;



        protected override void Start()
        {
            base.Start();

            _fixedMaxOffsetHeight = Background.GetComponent<RectTransform>().offsetMax.y;
        }

        private void Update()
        {
            Background.gameObject.GetComponent<Image>().enabled = Text != string.Empty;

            Background.GetComponent<RectTransform>().position -= new Vector3(0, Background.GetComponent<RectTransform>().offsetMax.y - _fixedMaxOffsetHeight, 0);
        }



        public void ShowInformation(BuildingFullDto info)
        {
            Debug.Log(info.IsArmyBuilding);

            Text =
                string.Format(
                    "{0}\n" +
                    (info.OwnerName == "no owner" ? "Без владельца\n" : "Владелец: '{1}'\n") +
                    (info.IsFinished ? "" : "Строители: " + info.Builders + "/" + info.MaxBuilders + "\n") +
                    "\nНезанятых людей: {2}",
                    info.Name,
                    info.OwnerName,
                    info.FreePeople)
                + (info.IsIncomeBuilding || info.IsWorkerBuilding ? "\n" : "")
                + (info.IsIncomeBuilding
                    ? string.Format(
                        "\nДоход: {0}",
                        info.Income.ToResourcesString())
                    : "")
                + (info.IsWorkerBuilding
                    ? string.Format(
                        "\nРабочие: {0}/{1}",
                        info.Workers,
                        info.MaxWorkers)
                    : "")
                + (info.IsArmyBuilding
                    ? string.Format(
                        "\n\nТренируемая армия: {0}" +
                        "\nВремя тренировки: {1}/{2} ({3})" +
                        "\nТребуется людей: {4}" +
                        "\nТребуется ресурсов: {5}",
                        info.CreatingArmy,
                        info.ArmyCreationTime.ToTimerString(),
                        info.ArmyCreationTimeMax.ToTimerString(),
                        info.ArmyQueueSize,
                        info.PeopleForArmy,
                        info.ArmyPrice.ToResourcesString())
                    : "")
                + (info.Armies.Any()
                    ? "\n\nАрмии: " + info.Armies.Aggregate("", (sum, army) => sum + ", " + army).Substring(2)
                    : "");
        }

        public void ShowInformation(ArmyDto[] info)
        {
            Text = info.Any()
                ? info.Aggregate(
                    "",
                    (sum, army) =>
                        sum
                        + ",\n"
                        + string.Format(
                            "{0} ({1}'s, {2})",
                            army.Name,
                            army.Owner,
                            army.LifePoints))
                    .Substring(2)
                : "";
        }

        public void ShowInformation(ArmyDto info)
        {
            Text = string.Format(
                "{0}\n\n" +
                "Боевая мощь: {1}\n" +
                "Владелец: '{2}'",
                info.Name, 
                info.LifePoints,
                info.Owner);
        }
    }
}