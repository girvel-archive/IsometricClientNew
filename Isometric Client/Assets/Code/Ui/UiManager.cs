using Assets.Code.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui
{
    public class UiManager : SingletonBehaviour<UiManager>
    {
        public InputField
            LoginInputField,
            PasswordInputField;

        public GameObject LoginForm;
    }
}