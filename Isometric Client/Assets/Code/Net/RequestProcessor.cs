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



        public Dictionary<string, object> Request(string type, Dictionary<string, object> args, params Type[] argsTypes)
        {
            var result = new Dictionary<string, object>();

            var serializer = JsonSerializer.Create(new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});

            var response = JToken.Parse(
                Connection.Request(
                    new JObject
                    {
                        {"type", type},
                        {"args", JToken.FromObject(args, serializer)}
                    }.ToString()))
                .ToObject<Dictionary<string, object>>();

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

            var i = 0;
            foreach (var pair in response)
            {
                result.Add(pair.Key, JToken.FromObject(pair.Value).ToObject(argsTypes[i]));
                i++;
            }

            return result;
        } 
    }
}
