namespace Dolany.Ai.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.ExceptionServices;

    using JetBrains.Annotations;

    public static class CommonUtil
    {
        private static Dictionary<string, string> AIConfig;

        [NotNull]
        [HandleProcessCorruptedStateExceptions]
        public static string GetConfig(string name)
        {
            if (AIConfig == null)
            {
                AIConfig = GetConfigDic();
            }

            return AIConfig.Keys.Contains(name) ? AIConfig[name] : string.Empty;
        }

        public static string GetConfig(string name, string defaltValue)
        {
            var value = GetConfig(name);
            return !string.IsNullOrEmpty(value) ? value : defaltValue;
        }

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
                    if (strs.IsNullOrEmpty() ||
                        strs.Length != 2)
                    {
                        continue;
                    }

                    dic.Add(strs[0], strs[1]);
                }

                return dic;
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            return objs == null || !objs.Any();
        }
    }
}
