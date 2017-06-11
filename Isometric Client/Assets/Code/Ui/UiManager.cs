using System.Linq;
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

        public Text ResourcesText;



        public void ShowResources(Isometric.Core.Resources resources)
        {
            ResourcesText.text =
                string.Format(
                    "Wood: {0}\nFood: {1}",
                    resources.GetResourcesArray().Select(r => (object) r.ToString()).ToArray());
        }
    }
}