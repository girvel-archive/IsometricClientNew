using Newtonsoft.Json;

namespace Assets.Code.Net
{
    public static class Serializer
    {
        public static JsonSerializer Current =
            JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Objects,
                    FloatParseHandling = FloatParseHandling.Decimal,
                });
    }
}