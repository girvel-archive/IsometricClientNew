using Assets.Code.Common;
using UnityEngine.UI;

namespace Assets.Code.Interface.Panels
{
    public class Panel<T> : SingletonBehaviour<T> where T : Panel<T>
    {
        public string Text
        {
            get { return GetComponent<Text>().text; }
            set { GetComponent<Text>().text = value; }
        }

        public Image Background;



        protected virtual void Update()
        {
            Background.gameObject.GetComponent<Image>().enabled = Text != string.Empty;
        }
    }
}