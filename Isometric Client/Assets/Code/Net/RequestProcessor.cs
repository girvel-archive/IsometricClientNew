using System;
using System.Collections.Generic;
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

            var i = 0;
            foreach (var pair in JToken.Parse(
                Connection.Request(
                    new JObject
                    {
                        {"type", type},
                        {"args", JToken.FromObject(args)}
                    }.ToString()))
                .ToObject<Dictionary<string, object>>())
            {
                result.Add(pair.Key, JToken.FromObject(pair.Value).ToObject(argsTypes[i]));
                i++;
            }

            return result;
        } 
    }
}
