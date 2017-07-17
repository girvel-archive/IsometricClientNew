using System.Collections.Generic;
using System.Linq;
using Isometric.Core.Vectors;

namespace Assets.Code.Common.Helpers
{
    public static class ResourcesHelper
    {
        public static string ToResourcesString(this float[] resources)
        {
            var result = "";
            var i = 0;
            foreach (var resource in resources.Where(r => r != 0))
            {
                result += resource + " " + Resources[i++];
            }

            return result;
        }

        private static readonly string[] Resources =
        {
            "Дерево",
            "Еда",
        };
    }
}