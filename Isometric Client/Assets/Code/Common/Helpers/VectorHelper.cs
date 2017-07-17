using Isometric.Core.Vectors;
using UnityEngine;

namespace Assets.Code.Common.Helpers
{
    public static class VectorHelper
    {
        public static Vector ToIsometricVector(this Vector2 vector2)
        {
            return new Vector((int)vector2.x, (int)vector2.y);
        }
        public static Vector2 ToVector2(this Vector vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
    }
}
