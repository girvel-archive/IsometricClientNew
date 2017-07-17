using Assets.Code.Common;
using Assets.Code.Interface.Table;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interface.Panels
{
    public class TipPanel : Panel<TipPanel>
    {
        private int _currentTipIndex;



        public Text TipIndexText;
        
        public string[] Tips { get; set; }

        public int CurrentTipIndex
        {
            get { return _currentTipIndex; }
            private set
            {
                _currentTipIndex = value;

                Text = Tips[CurrentTipIndex];
                TipIndexText.text = string.Format("{0}/{1}", CurrentTipIndex + 1, Tips.Length);
            }
        }


        protected override void Start()
        {
            base.Start();

            Tips = new[]
            {
                "Первая подсказка",
                "Вторая подсказка",
            };

            if (Settings.Current.ShowTips)
            {
                ActionProcessor.Current.AddActionToQueue(() => CurrentTipIndex = 0);
            }
            else
            {
                Stop();
            }
        }


        public void NextTip()
        {
            CurrentTipIndex = (CurrentTipIndex + 1) % Tips.Length;
        }

        public void PreviousTip()
        {
            CurrentTipIndex = (CurrentTipIndex - 1 + Tips.Length) % Tips.Length;
        }

        public void Stop()
        {
            Settings.Current.ShowTips = false;

            Ui.Current.TipForm.SetActive(false);
        }
    }
}