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
            ActionProcessor.Current.AddActionToQueue(
                () =>
                {
                    try
                    {
                        NetManager.Current.Run(
                            Ui.Current.LoginInputField.text,
                            Ui.Current.PasswordInputField.text,
                            IPAddress.Parse(Ui.Current.IpInputField.text));

                        Ui.Current.GameUiForm.SetActive(true);
                        Ui.Current.LoginForm.SetActive(false);
                    }
                    catch (ConnectionToServerException)
                    {
                        Ui.Current.LoginStatusText.text = "Can not connect to server";
                    }
                    catch (LoginException)
                    {
                        Ui.Current.LoginStatusText.text = "Wrong login or password";
                    }
                });

        }
    }
}