using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            
            using (var reader = File.OpenText("Assets/Content/tips.txt"))
            {
                Tips = reader
                    .ReadToEnd()
                    .Split(new[] {"\n-"}, StringSplitOptions.RemoveEmptyEntries);
            }

            if (Settings.Current.ShowTips)
            {
                CurrentTipIndex = 0;    
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