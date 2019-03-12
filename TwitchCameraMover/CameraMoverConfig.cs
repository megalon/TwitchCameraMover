using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TwitchCameraMover
{
    public class CameraMoverConfig
    {

        public float MaxCameraDist { get; set; }
        public bool CommandsRestricted { get; set; }
        public UnityEngine.Vector3 LookAt { get; set; }
        public bool AllowForMods { get; set; }
        public bool AllowForVips { get; set; }
        public bool AllowForSubs { get; set; }

        public static CameraMoverConfig Initialize()
        {
            var conf = new CameraMoverConfig() {
                MaxCameraDist = 1000,
                CommandsRestricted = true,
                AllowForMods = true,
                AllowForSubs = true,
                AllowForVips = true,
                LookAt = new UnityEngine.Vector3(0, 1, 0)
            };

            var filePath = Path.Combine(Environment.CurrentDirectory, "UserData", "TwitchCameraMover.json");
            if (!File.Exists(filePath))
            {

                //var txt = "";
                //foreach (var fi in typeof(CameraMoverConfig).GetFields())
                //{
                //    txt += fi.Name + "=" + fi.GetValue(this) + "\r\n";
                //}

                //File.WriteAllText(filePath, txt);
                //return;


                var js = Newtonsoft.Json.JsonConvert.SerializeObject(conf, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, js);
                return conf;
            }

            var contents = File.ReadAllText(filePath);

            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<CameraMoverConfig>(contents) as CameraMoverConfig;
            if (config == null)
            {
                Plugin.Log("Could not deserialize json for config.", Plugin.LogLevel.Error);
                return conf;
            }

            return config;
        }


    }

    public static class CamMovExtensions
    {
        public static string ToString(this UnityEngine.Vector3 vec) => $"{vec.x} {vec.y} {vec.z}";
    }
}
