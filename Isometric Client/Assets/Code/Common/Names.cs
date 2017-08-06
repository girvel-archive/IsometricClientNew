using System.Collections.Generic;

namespace Assets.Code.Common
{
    public class Names
    {
        public static readonly Dictionary<string, string>
            RealBuildingsNames = new Dictionary<string, string>
            {
                {"Plain", "Равнина"},
                {"Forest", "Лес"},
                {"Underground house", "Землянка"},
                {"Wooden house", "Деревянный дом"},
                {"Sawmill", "Лесопилка"},
                {"Mill", "Мельница"},
                {"Barracks", "Казармы пехоты"},
                {"Barracks of heavy infantry", "Казармы тяжелой пехоты"},
                {"Spirit house", "Дом духов"},
                {"Bakery", "Пекарня"},
                {"Coal mine", "Угольная шахта"},
                {"Field", "Поле"},
                {"Hunter's shack", "Хижина охотника"},
                {"Workshop", "Мастерская"},
            },
            RealArmiesNames = new Dictionary<string, string>
            {
                {"Infantry", "Пехота"},
                {"Heavy infantry", "Тяжелая пехота"},
            };

        public static readonly string[] ResourcesNames =
        {
            "Дерево",
            "Еда",
            "Cырая еда",
            "Инструменты",
            "Зерно",
            "Уголь",
        };
    }
}