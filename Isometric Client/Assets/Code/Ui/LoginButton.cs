using UnityEngine;

namespace Assets.Code.Ui
{
    public class LoginButton : MonoBehaviour
    {
        public void OnClick()
        {
            NetManager.Current.Run(
                UiManager.Current.LoginInputField.text,
                UiManager.Current.PasswordInputField.text);

            UiManager.Current.LoginForm.SetActive(false);
        }
    }
}