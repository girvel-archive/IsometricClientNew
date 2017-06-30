using Assets.Code.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Interface
{
    public class Ui : SingletonBehaviour<Ui>
    {
        public InputField
            IpInputField,
            LoginInputField,
            PasswordInputField;

        public GameObject 
            LoginForm, 
            GameUiForm; 

        public Text ResourcesText;
    }
}