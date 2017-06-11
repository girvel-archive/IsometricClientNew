using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Isometric.Core;
using UnityEngine;

namespace Assets.Code.Common
{
    public static class VectorHelper
    {
        public static Vector ToIsometricVector(this Vector2 vector2)
        {
            return new Vector((int)vector2.x, (int)vector2.y);
        } 
    }
}
