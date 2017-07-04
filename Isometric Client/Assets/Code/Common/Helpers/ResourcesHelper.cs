using System.Linq;
using Isometric.Core;

namespace Assets.Code.Common.Helpers
{
    public static class ResourcesHelper
    {
        public static string ToFormattedString(this float[] resources)
        {
            var result = "";
            var i = 0;
            foreach (var resource in resources.Where(r => r != 0))
            {
                result += resource + " " + ((ResourceType) i++).ToString("F");
            }

            return result;
        }
    }
}