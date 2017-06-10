using UnityEngine;

namespace Assets.Code.Common
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        public static T Current { get; private set; }



        protected virtual void Start()
        {
            Current = (T) this;
        }
    }
}