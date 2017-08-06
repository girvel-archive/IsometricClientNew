using UnityEngine;

namespace Assets.Code.Building
{
    [ExecuteInEditMode]
    public class IsometricController : MonoBehaviour
    {
        public Vector2 IsometricPosition = Vector2.zero;

        protected Vector2 LastIsometricPosition = Vector2.zero;
        protected Vector2 LastPosition;
        
        public static readonly Vector2 DefaultPlatformSize = new Vector2(0.56f, 0.26f);

        public static Vector2 IsometricPositionToNormal(Vector2 isometricPosition, Vector2 platformSize)
        {
            var roundx = Mathf.RoundToInt(isometricPosition.x /
                (0.5f * DefaultPlatformSize.x * platformSize.x));

            var roundy = Mathf.RoundToInt(isometricPosition.y /
                (0.5f * DefaultPlatformSize.y * platformSize.y));

            return new Vector2(
                (roundy - roundx) * 0.5f,
                (roundx + roundy) * 0.5f);
        }

        protected void Start()
        {
            LastPosition = transform.position;
        }

        protected virtual void Update()
        {
            if (IsometricPosition != LastIsometricPosition)
            {
                GetComponent<SpriteRenderer>().sortingOrder = -2 * (int)(IsometricPosition.x + IsometricPosition.y);

                LastIsometricPosition = IsometricPosition;
            }
            else if ((Vector2)transform.position != LastPosition)
            {
                IsometricPosition = IsometricPositionToNormal(transform.position, transform.localScale);
            }

            transform.position = new Vector2(
                0.5f * DefaultPlatformSize.x * transform.localScale.x *
                    (IsometricPosition.y - IsometricPosition.x),
                0.5f * DefaultPlatformSize.y * transform.localScale.y *
                    (IsometricPosition.y + IsometricPosition.x));

            LastPosition = transform.position;
        }
    }
}