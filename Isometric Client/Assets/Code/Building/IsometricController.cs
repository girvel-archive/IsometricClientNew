using UnityEngine;

namespace Assets.Code.Building
{
    [ExecuteInEditMode]
    public class IsometricController : MonoBehaviour
    {
        public Vector2 IsometricPosition = Vector2.zero;

        protected Vector2 LastIsometricPosition = Vector2.zero;
        protected Vector2 LastPosition;
        
        public static readonly Vector2 DefaultPlatformSize = new Vector2(1.32f, 0.64f);

        public static Vector2 NormalPositionToIsometric(Vector2 normalPosition, Vector2 scale)
        {
            var roundx = Mathf.RoundToInt(normalPosition.x /
                (0.5f * DefaultPlatformSize.x * scale.x));

            var roundy = Mathf.RoundToInt(normalPosition.y /
                (0.5f * DefaultPlatformSize.y * scale.y));

            return new Vector2(
                (roundy - roundx) * 0.5f,
                (roundx + roundy) * 0.5f);
        }

        public static Vector2 IsometricPositionToNormal(Vector2 isometricPosition, Vector2 scale)
        {
            return new Vector2(
                0.5f * DefaultPlatformSize.x * scale.x *
                (isometricPosition.y - isometricPosition.x),
                0.5f * DefaultPlatformSize.y * scale.y *
                (isometricPosition.y + isometricPosition.x));
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
                IsometricPosition = NormalPositionToIsometric(transform.position, transform.localScale);
            }

            transform.position = IsometricPositionToNormal(IsometricPosition, transform.localScale);

            LastPosition = transform.position;
        }
    }
}