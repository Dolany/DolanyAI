namespace Dolany.Ai.MQ
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.ExceptionServices;

    using Dolany.Ai.Util;

    public static class Utility
    {
        private static string AuthCode;
        private static Dictionary<string, string> AIConfig;
        public static long SelfQQNum => long.Parse(GetConfig(nameof(SelfQQNum)));

        public static string GetAuthCode()
        {
            if (!AuthCode.IsNullOrEmpty())
            {
                return AuthCode;
            }

            const string logPath = "./logs/";
            var dir = new DirectoryInfo(logPath);

            foreach (var file in dir.GetFiles().OrderByDescending(p => p.CreationTime))
            {
                var authLine = string.Empty;
                using (var reader = new StreamReader(file.FullName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("\"AuthCode\":"))
                        {
                            authLine = line;
                        }
                    }
                }

                if (authLine.IsNullOrEmpty())
                {
                    continue;
                }

                var strs = authLine.Split('\"');
                AuthCode = strs[3];
                break;
            }

            return AuthCode;
        }

        [HandleProcessCorruptedStateExceptions]
        public static string GetConfig(string name)
        {
            try
            {
                if (AIConfig == null)
                {
                    AIConfig = GetConfigDic();
                }

                return AIConfig.Keys.Contains(name) ? AIConfig[name] : string.Empty;
            }
            catch (Exception ex)
            {
                MahuaModule.RuntimeLogger.Log(ex);
                return string.Empty;
            }
        }

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
    }
}
