using System.Linq;
using Assets.Code.Common;
using Assets.Code.Net;
using UnityEngine.UI;

namespace Assets.Code.Ui
{
    public class InformationPanel : SingletonBehaviour<InformationPanel>
    {
        public string Text
        {
            get { return GetComponent<Text>().text; }
            set { GetComponent<Text>().text = value; }
        }



        public void ShowInformation(NetManager.BuildingInfo info)
        {
            Text =
                string.Format(
                    "{0}\n" +
                    "Owner: '{1}'\n" +
                    (info.Finished ? "" : "Builders: " + info.Builders + "/" + info.MaxBuilders + "\n") +
                    "\nPeople: {2}\n",
                    info.Name,
                    info.OwnerName,
                    info.People)
                + (info.IsIncomeBuilding
                    ? string.Format(
                        "\nIncome: {0}\n" +
                        "Workers: {1}/{2}\n",
                        info.Income.ToFormattedString(),
                        info.Workers,
                        info.MaxWorkers)
                    : "");
        }
    }
}