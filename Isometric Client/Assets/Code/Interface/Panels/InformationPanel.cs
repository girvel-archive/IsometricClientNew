using System.Collections.Generic;
using System.Linq;
using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Interface.Table;
using Isometric.Dtos;

namespace Assets.Code.Interface.Panels
{
    public class InformationPanel : Panel<InformationPanel>
    {
        public void ShowInformation(BuildingFullDto info)
        {
            Text =
                string.Format(
                    "{0}\n" +
                    (info.OwnerName == "no owner" ? "Без владельца\n" : "Владелец: '{1}'\n") +
                    (info.IsFinished ? "" : "Не достроено\nСтроители: " + info.Builders + "/" + info.MaxBuilders + "\n") +
                    "\nНезанятых людей: {2}",
                    Names.RealBuildingsNames[info.Name],
                    info.OwnerName,
                    info.FreePeople)
                + (info.IsIncomeBuilding || info.IsWorkerBuilding ? "\n" : "")
                + (info.IsIncomeBuilding
                    ? (info.Income.Where((t, i) => t != info.LastIncome[i]).Any() // if income is not maximal
                        ? string.Format( 
                            "\nМаксимальный доход: {0}"
                            + "\nТекущий доход: {1}",
                            info.Income.ToResourcesString(),
                            info.LastIncome.ToResourcesString())
                        : "\nДоход:" + info.Income.ToResourcesString())
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
                            "{0} ({1}, {2})",
                            Names.RealArmiesNames[army.Name],
                            army.Owner,
                            army.LifePoints))
                    .Substring(2)
                : "";
        }

        public void ShowInformation(ArmyDto info)
        {
            Text = string.Format(
                "{0}\n\n"
                + ArmorTypes[info.ArmorType]
                + "\nОчки здоровья: {1}"
                + "\nУрон: {2}"
                + (info.BonusDamage == 0
                    ? ""
                    : string.Format(
                        " ({0}{1} урона по {2})",
                        info.BonusDamage > 0 ? "+" : "-",
                        info.BonusDamage,
                        ArmorTypes1[info.BonusArmorType]))
                + "\nВладелец: '{3}'" 
                + "\n{4}",
                Names.RealArmiesNames[info.Name],
                info.LifePoints,
                info.Damage,
                info.Owner,
                Tasks[info.Task]);
        }



        private static readonly string[] ArmorTypes =
        {
            "Без брони",
            "Здание",
            "Легкая броня",
            "Средняя броня",
            "Тяжелая броня",
        };

        private static readonly string[] ArmorTypes1 =
        {
            "юнитам без брони",
            "зданиям",
            "юнитам с легкой броней",
            "юнитам с средней броней",
            "юнитам с тяжелой броней",
        };

        private static readonly Dictionary<string, string> Tasks = new Dictionary<string, string>
        {
            {"DestroyingTask", "Разрушает здание"},
            {"MovingTask", "Двигается"},
            {"", ""},
        };  
    }
}