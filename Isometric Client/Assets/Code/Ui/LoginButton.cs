using System.Net;
using Assets.Code.Net;
using UnityEngine;

namespace Assets.Code.Ui
{
    public class LoginButton : MonoBehaviour
    {
        public void OnClick()
        {
            NetManager.Current.Run(
                UiManager.Current.LoginInputField.text,
                UiManager.Current.PasswordInputField.text,
                IPAddress.Parse(UiManager.Current.IpInputField.text));

            UiManager.Current.LoginForm.SetActive(false);
            UiManager.Current.GameUiForm.SetActive(true);
        }
    }
}