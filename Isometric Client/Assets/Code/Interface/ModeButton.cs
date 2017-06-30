using UnityEngine;

namespace Assets.Code.Interface
{
    public class ModeButton : MonoBehaviour
    {
        public UiMode Mode;



        public void OnClick()
        {
            GameUi.Current.SetMode(Mode);
        }
    }
}