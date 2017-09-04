using System;
using System.Linq;
using System.Net;
using Assets.Code.Common;
using Assets.Code.Net;
using UnityEngine;

namespace Assets.Code.Interface.Buttons
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
                        IPAddress ip;

                        NetManager.Current.Run(
                            Ui.Current.LoginInputField.text,
                            Ui.Current.PasswordInputField.text,
                            IPAddress.TryParse(Ui.Current.IpInputField.text, out ip)
                                ? ip
                                : Dns.GetHostAddresses(Ui.Current.IpInputField.text).First());

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
                    catch (InvalidOperationException)
                    {
                        Ui.Current.LoginStatusText.text = "Can not get host address";
                    }
#if !DEBUG
                    catch (Exception)
                    {
                        Ui.Current.LoginStatusText.text = "Something went wrong";
                    }
#endif
                });

        }
    }
}