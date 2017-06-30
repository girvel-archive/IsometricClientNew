using System.Net;
using Assets.Code.Common;
using Assets.Code.Net;
using UnityEngine;

namespace Assets.Code.Interface
{
    public class LoginButton : MonoBehaviour
    {
        public void OnClick()
        {
            Ui.Current.GameUiForm.SetActive(true);

            ActionProcessor.Current.AddActionToQueue(
                () =>
                {
                    NetManager.Current.Run(
                        Ui.Current.LoginInputField.text,
                        Ui.Current.PasswordInputField.text,
                        IPAddress.Parse(Ui.Current.IpInputField.text));

                    Ui.Current.LoginForm.SetActive(false);
                });

        }
    }
}