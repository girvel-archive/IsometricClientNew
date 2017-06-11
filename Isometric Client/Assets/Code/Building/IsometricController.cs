using UnityEngine;

namespace Assets.Code.Building
{
    [ExecuteInEditMode]
    public class IsometricController : MonoBehaviour
    {
        public Vector2 IsometricPosition = Vector2.zero;
        public Vector2 PlatformSize = Vector2.one;

        protected Vector2 LastIsometricPosition = Vector2.zero;
        protected Vector2 LastPosition;
        
        public static readonly Vector2 DefaultPlatformSize = new Vector2(0.56f, 0.26f);

        protected void Start()
        {
            LastPosition = transform.position;
        }

        protected virtual void Update()
        {
            IsometricPositionRefresh();
        }

        private void IsometricPositionRefresh()
        {
            if (IsometricPosition != LastIsometricPosition)
            {
                transform.position = new Vector2(
                    0.5f * DefaultPlatformSize.x * transform.localScale.x *
                        (IsometricPosition.y - IsometricPosition.x),
                    0.5f * DefaultPlatformSize.y * transform.localScale.y *
                        (IsometricPosition.y + IsometricPosition.x));

                GetComponent<SpriteRenderer>().sortingOrder = -(int) (IsometricPosition.x + IsometricPosition.y);

                LastIsometricPosition = IsometricPosition;
            }
            else if ((Vector2)transform.position != LastPosition)
            {
                var roundx = Mathf.RoundToInt(transform.position.x /
                                     (0.5f * DefaultPlatformSize.x * transform.localScale.x));

                var roundy = Mathf.RoundToInt(transform.position.y /
                                     (0.5f * DefaultPlatformSize.y * transform.localScale.y));

                IsometricPosition = new Vector2(
                    (roundy - roundx) * 0.5f,
                    (roundx + roundy) * 0.5f);

                transform.position = new Vector2(
                    0.5f * DefaultPlatformSize.x * transform.localScale.x *
                        (IsometricPosition.y - IsometricPosition.x),
                    0.5f * DefaultPlatformSize.y * transform.localScale.y *
                        (IsometricPosition.y + IsometricPosition.x));
            }
            LastPosition = transform.position;
        }
    }
}