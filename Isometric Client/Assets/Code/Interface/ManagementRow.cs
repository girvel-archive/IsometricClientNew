using Assets.Code.Common.Helpers;
using Assets.Code.Interface.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interface
{
    public class ManagementRow : MonoBehaviour
    {
        private int _workersNow, _workersMax;



        public Text NameText, IncomeText, WorkersText;

        public WorkersNumberButton PlusButton, MinusButton;



        public string RealName { get; set; }

        public string Name
        {
            get { return NameText.text; }
            set { NameText.text = value; }
        }

        public string Income
        {
            set { IncomeText.text = value; }
        }

        public int WorkersNow
        {
            get { return _workersNow; }
            set
            {
                _workersNow = value;
                WorkersText.text = WorkersNow + "/" + WorkersMax;
            }
        }

        public int WorkersMax
        {
            get { return _workersMax; }
            set
            {
                _workersMax = value;
                WorkersText.text = WorkersNow + "/" + WorkersMax;
            }
        }
    }
}