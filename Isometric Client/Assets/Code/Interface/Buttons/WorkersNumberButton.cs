using Assets.Code.Net;
using UnityEngine;

namespace Assets.Code.Interface.Buttons
{
    public class WorkersNumberButton : MonoBehaviour
    {
        public int WorkersDelta;

        public ManagementRow ManagementRow;



        public void OnClick()
        {
            ManagementRow.WorkersNow += NetManager.Current.AddWorkersForPrototype(ManagementRow.RealName, WorkersDelta);
            GameUi.Current.Refresh();
        }
    }
}