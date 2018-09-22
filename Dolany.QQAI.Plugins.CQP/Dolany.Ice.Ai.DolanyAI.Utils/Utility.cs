using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;

namespace Dolany.Ice.Ai.DolanyAI.Utils
{
    public static class Utility
    {
        private static string AuthCode;

        public static long DeveloperNumber => long.Parse(GetConfig(nameof(DeveloperNumber)));
        public static long SysMsgNumber => long.Parse(GetConfig(nameof(SysMsgNumber)));
        public static long SelfQQNum => long.Parse(GetConfig(nameof(SelfQQNum)));

        private static Dictionary<string, string> AIConfig;

        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            return objs == null || !objs.Any();
        }

        public static (int hour, int minute)? GenTimeFromStr(string timeStr)
        {
            var strs = timeStr.Split(':', '：');
            if (strs.Length != 2)
            {
                return null;
            }

            if (!int.TryParse(strs[0], out var hour))
            {
                return null;
            }

            if (!int.TryParse(strs[1], out var minute))
            {
                return null;
            }

            return (hour, minute);
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

                return AIConfig.Keys.Contains(name) ? AIConfig[name] : "";
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                return string.Empty;
            }
        }

        public static string GetConfig(string name, string defaltValue)
        {
            var value = GetConfig(name);
            return !string.IsNullOrEmpty(value) ? value : defaltValue;
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

        public static Dictionary<int, string> LoadFortuneImagesConfig()
        {
            var dic = new Dictionary<int, string>();
            var configFile = new FileInfo("FortuneImagesConfig.ini");
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

                    dic.Add(int.Parse(strs[0]), strs[1]);
                }

                return dic;
            }
        }

        public static T Clone<T>(this T obj) where T : class, new()
        {
            var type = obj.GetType();
            var copyT = new T();
            foreach (var prop in type.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(copyT, prop.GetValue(obj));
                }
            }

            return copyT;
        }

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
                var authLine = "";
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

        public static string UrlCharConvert(string name)
        {
            var result = string.Empty;
            var builder = new StringBuilder();
            builder.Append(result);
            foreach (var c in name)
            {
                if (IsAsciiChar(c))
                {
                    builder.Append(c);
                    continue;
                }

                builder.Append(@"%" + BitConverter.ToString(Encoding.UTF8.GetBytes(new[] { c })).Replace("-", "%"));
            }
            result = builder.ToString();

            return result;
        }

        private static bool IsAsciiChar(char c)
        {
            return c >= 0x20 && c <= 0x7e;
        }

        public static string ToCommonString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static int RandInt(int MaxValue)
        {
            const decimal _base = int.MaxValue;
            var bytes = new byte[4];
            RngCsp.GetBytes(bytes);

            return (int)(Math.Abs(BitConverter.ToInt32(bytes, 0)) / _base * MaxValue);
        }

        public static string ParsePicName(string msg)
        {
            var picIdx = msg.IndexOf("[QQ:pic=", StringComparison.Ordinal);
            if (picIdx < 0)
            {
                return "";
            }

            var startIdx = picIdx + "[QQ:pic=".Length;
            var endIdx = msg.IndexOf("]", startIdx, StringComparison.Ordinal);
            var picName = msg.Substring(startIdx, endIdx - startIdx);

            return picName;
        }
    }
}