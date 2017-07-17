using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Assets.Code.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Code.Saving
{
    public class SavingManager : SingletonBehaviour<SavingManager>
    {
        protected override void Start()
        {
            base.Start();

#if !DEBUG
            Load();
#endif
        }

        private void Update()
        {
            _savingPeriod -= TimeSpan.FromSeconds(Time.deltaTime);

            if (_savingPeriod < TimeSpan.Zero)
            {
                _savingPeriod = TimeSpan.FromSeconds(1);

                Save();
            }
        }
        private TimeSpan _savingPeriod = TimeSpan.FromSeconds(1);



        public void Load()
        {
            using (var stream = File.OpenRead("settings.json"))
            using (var reader = new StreamReader(stream))
            {
                string data;
                var loadedData = JObject.Parse(data = reader.ReadToEnd());
                Debug.Log(data);

                Settings.Current = loadedData["settings"].ToObject<Settings>();
            }
        }

        public void Save()
        {
            using (var stream = File.OpenWrite("settings.json"))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(
                    JObject.FromObject(
                        new Dictionary<string, object>
                        {
                            {"settings", Settings.Current},
                        }).ToString());
            }
        }
    }
}