using System.Collections.Generic;
using Isometric.Core;

namespace Assets.Code.Net
{
    public class IsometricRequestProcessor : RequestProcessor
    {
        public IsometricRequestProcessor(Connection connection) : base(connection) { }


        public Resources GetResources()
        {
            return (Isometric.Core.Resources) Request(
                "get resources",
                new Dictionary<string, object>())["resources"];
        }
    }
}