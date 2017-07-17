using Assets.Code.Interface.Panels;
using Assets.Code.Interface.Table;
using UnityEngine;

namespace Assets.Code.Interface.Buttons
{
    public class NextTipButton : MonoBehaviour
    {
        public void OnClick()
        {
            TipPanel.Current.NextTip();
        }
    }
}