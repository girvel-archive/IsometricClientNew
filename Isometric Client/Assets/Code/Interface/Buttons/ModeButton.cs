using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interface
{
    public class ModeButton : GameUiButton
    {
        public UiMode Mode;

        public Image ActiveFrame;



        protected override void OnHotkeyPress()
        {
            OnClick();
        }



        public void OnClick()
        {
            GameUi.Current.SetMode(Mode);
        }

        public void SetActiveFrame(bool active)
        {
            ActiveFrame.gameObject.SetActive(active);
        }
    }
}