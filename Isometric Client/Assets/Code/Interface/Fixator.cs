using UnityEngine;

namespace Assets.Code.Interface
{
    public class Fixator : MonoBehaviour
    {
        private float _fixedMaxOffsetHeight;
        
        private void Start()
        {
            _fixedMaxOffsetHeight = GetComponent<RectTransform>().offsetMax.y;
        }

        private void Update()
        {
            GetComponent<RectTransform>().position 
                -= new Vector3(0, GetComponent<RectTransform>().offsetMax.y - _fixedMaxOffsetHeight, 0);
        }
    }
}