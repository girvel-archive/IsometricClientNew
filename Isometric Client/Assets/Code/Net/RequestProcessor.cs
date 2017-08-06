using System;
using System.Collections.Generic;
using Assets.Code.Interface;
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
            getResponse: Dictionary<string, object> response;
            var i = 0;
            try
            {
                response = JToken.Parse(
                    Connection.Request(
                        new JObject
                        {
                            {"type", type},
                            {"args", JToken.FromObject(args ?? new Dictionary<string, object>(), Serializer.Current)}
                        }.ToString()))
                    .ToObject<Dictionary<string, object>>(Serializer.Current);
            }
            catch (JsonReaderException)
            {
                i++;
                if (i <= 10)
                {
                    goto getResponse;
                }
                else
                {
                    throw;
                }
            }
            

            object errorType;
            if (response.TryGetValue("error type", out errorType))
            {
                var errorTypeString = (string) errorType;

                if (errorTypeString == "permission")
                {
                    GameUi.Current.ShowMessage("Ошибка на сервере: недостаточно прав", TimeSpan.FromSeconds(4));
                    throw new PermissionDeniedException();
                }

                GameUi.Current.ShowMessage("Ошибка на сервере", TimeSpan.FromSeconds(4));
                throw new Exception("Server error of type " + errorTypeString);
            }

            return response;
        } 
    }
}
