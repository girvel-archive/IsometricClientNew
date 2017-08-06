using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Isometric.Core.Vectors;

namespace Assets.Code.Common.Helpers
{
    public static class ResourcesHelper
    {
        public static string ToResourcesString(this float[] resources)
        {
            var result = "";
            if (resources != null)
            {
                for (var i = 0; i < resources.Length; i++)
                {
                    var r = Math.Round(resources[i]);

                    var currentString = ", " + r + " " + Names.ResourcesNames[i];

                    if (r > 0)
                    {
                        result += currentString;
                    }
                    else if (r < 0)
                    {
                        result = currentString + result;
                    }
                }
            }

            return result == "" ? "-" : result.Substring(2);
        }

        public static float[] Added(this float[] r1, float[] r2)
        {
            var result = (float[]) r1.Clone();

            for (var i = 0; i < result.Length; i++)
            {
                result[i] += r2[i];
            }

            return result;
        }
    }
}