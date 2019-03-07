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
        public CameraMoverConfig()
        {
            MaxCameraDist = 1000;
            CommandsRestricted = true;
            AllowForMods = true;
            AllowForSubs = true;
            AllowForVips = true;
            LookAt = new UnityEngine.Vector3(0, 1, 0);

            var filePath = Path.Combine(Environment.CurrentDirectory, "UserData", "TwitchCameraMover.cnf");
            if (!File.Exists(filePath))
            {

                var txt = "";
                foreach (var fi in typeof(CameraMoverConfig).GetFields())
                {
                    txt += fi.Name + "=" + fi.GetValue(this) + "\r\n";
                }

                File.WriteAllText(filePath, txt);
                return;
            }

            var contents = File.ReadAllText(filePath);
            var lines = contents.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            //temp lazy workaround without using a parser
            foreach (var ln in lines)
            {
                var line = ln.Trim();
                var eqidx = line.IndexOf('=');
                if (eqidx == -1) continue;

                var key = line.Substring(0, eqidx);
                var prop = typeof(CameraMoverConfig).GetProperty(key);
                if (prop == null) continue;

                var val = line.Substring(Math.Min(eqidx + 1, line.Length));

                if (prop.Name == "LookAt")
                {
                    var splits = val.Split(' ');
                    if (splits.Length != 3) continue;

                    var vec = new UnityEngine.Vector3(float.Parse(splits[0]), float.Parse(splits[1]), float.Parse(splits[2]));
                    prop.SetValue(this, vec);
                    continue;
                }
                switch (Type.GetTypeCode(prop.PropertyType))
                {
                    case TypeCode.Boolean:
                        prop.SetValue(this, val.ToLower() == "true");
                        break;
                    case TypeCode.Single:
                    case TypeCode.Double:
                        prop.SetValue(this, float.Parse(val));
                        break;
                    case TypeCode.Int32:
                        prop.SetValue(this, int.Parse(val));
                        break;
                    case TypeCode.String:
                        prop.SetValue(this, val);
                        break;
                    default:
                        prop.SetValue(this, prop.InvokeConstructor(val));
                        break;
                }
            }
        }

        public float MaxCameraDist { get; set; }
        public bool CommandsRestricted { get; set; }
        public UnityEngine.Vector3 LookAt { get; set; }
        public bool AllowForMods { get; set; }
        public bool AllowForVips { get; set; }
        public bool AllowForSubs { get; set; }

    }

    public static class CamMovExtensions
    {
        public static string ToString(this UnityEngine.Vector3 vec) => $"{vec.x} {vec.y} {vec.z}";
    }
}
