using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Dolany.Ai.Common
{
    public class Configger
    {
        private readonly Dictionary<string, string> AIConfig;

        public static Configger Instance { get; } = new Configger();

        private Configger()
        {
            AIConfig = GetConfigDic();
        }

        public string this[string key] => AIConfig.Keys.Contains(key) ? AIConfig[key] : string.Empty;

        [NotNull]
        private static Dictionary<string, string> GetConfigDic()
        {
            var configFile = new FileInfo("AIConfig.ini");
            var dic = new Dictionary<string, string>();
            using (var reader = new StreamReader(configFile.FullName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var strs = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.IsNullOrEmpty() || strs.Length != 2)
                    {
                        continue;
                    }

                    dic.Add(strs[0], strs[1]);
                }

                return dic;
            }
        }
    }
}
