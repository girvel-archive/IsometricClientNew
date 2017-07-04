using Assets.Code.Common;
using Assets.Code.Common.Helpers;
using Assets.Code.Net;
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



        public void ShowInformation(NetManager.BuildingInfo info)
        {
            Text =
                string.Format(
                    "{0}\n" +
                    (info.OwnerName == "no owner" ? "No owner" : "Owner: '{1}'\n") +
                    (info.Finished ? "" : "Builders: " + info.Builders + "/" + info.MaxBuilders + "\n") +
                    "\nPeople: {2}\n",
                    info.Name,
                    info.OwnerName,
                    info.People)
                + (info.IsIncomeBuilding || info.IsWorkerBuilding ? "\n" : "")
                + (info.IsIncomeBuilding
                    ? string.Format(
                        "Income: {0}\n",
                        info.Income.ToFormattedString())
                    : "")
                + (info.IsWorkerBuilding
                    ? string.Format(
                        "Workers: {0}/{1}\n",
                        info.Workers,
                        info.MaxWorkers)
                    : "");
        }
    }
}