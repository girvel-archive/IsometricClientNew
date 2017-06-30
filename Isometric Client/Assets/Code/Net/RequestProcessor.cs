using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assets.Code.Net
{
    public class RequestProcessor
    {
        public Connection Connection { get; set; }



        public RequestProcessor(Connection connection)
        {
            Connection = connection;
        }



        public Dictionary<string, object> Request(string type, Dictionary<string, object> args = null)
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});

            var response = JToken.Parse(
                Connection.Request(
                    new JObject
                    {
                        {"type", type},
                        {"args", JToken.FromObject(args ?? new Dictionary<string, object>(), serializer)}
                    }.ToString()))
                .ToObject<Dictionary<string, object>>(Serializer.Current);

            object errorType;
            if (response.TryGetValue("error type", out errorType))
            {
                var errorTypeString = (string) errorType;

                if (errorTypeString == "permission")
                {
                    throw new PermissionDeniedException();
                }

                throw new Exception("Server error of type " + errorTypeString);
            }

            return response;
        } 
    }
}
