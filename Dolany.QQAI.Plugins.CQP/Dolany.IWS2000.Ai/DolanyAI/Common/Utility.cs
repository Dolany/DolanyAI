using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using Dolany.IWS2000.Ai.MahuaApis;
using Dolany.IWS2000.Ai.DolanyAI.Db;

namespace Dolany.IWS2000.Ai.DolanyAI
{
    public static class Utility
    {
        private static Dictionary<Type, object> SinglonMap;
        private static string AuthCode;

        public static long DeveloperNumber => 1458978159;

        public static void SendMsgToDeveloper(string msg)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = DeveloperNumber,
                Type = MsgType.Private,
                Msg = msg
            });
        }

        public static void SendMsgToDeveloper(Exception ex)
        {
            SendMsgToDeveloper(ex.Message + '\r' + ex.StackTrace);
        }

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

        public static void SetConfig(string name, string value)
        {
            using (var db = new AIDatabase())
            {
                var query = db.AIConfig.Where(p => p.Key == name);
                if (query.IsNullOrEmpty())
                {
                    db.AIConfig.Add(new AIConfig
                    {
                        Id = Guid.NewGuid().ToString(),
                        Key = name,
                        Value = value
                    });
                }
                else
                {
                    var config = query.First();
                    config.Value = value;
                }
                db.SaveChanges();
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public static string GetConfig(string name)
        {
            try
            {
                using (var db = new AIDatabase())
                {
                    var query = db.AIConfig.Where(p => p.Key == name);
                    return query.IsNullOrEmpty() ? string.Empty : query.First().Value;
                }
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
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            SetConfig(name, defaltValue);
            return defaltValue;
        }

        public static T Instance<T>() where T : class, new()
        {
            if (SinglonMap == null)
            {
                SinglonMap = new Dictionary<Type, object>();
            }

            var type = typeof(T);
            if (SinglonMap.Keys.Contains(type))
            {
                return SinglonMap[type] as T;
            }

            var value = new T();
            SinglonMap.Add(type, value);
            return value;
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

        public static MemberRoleCache GetMemberInfo(GroupMsgDTO MsgDTO)
        {
            return GroupMemberInfoCacher.GetMemberInfo(MsgDTO);
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

        public static ImageCacheModel ReadImageCacheInfo(FileInfo file)
        {
            using (var reader = new StreamReader(file.FullName))
            {
                var model = new ImageCacheModel();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var strs = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.IsNullOrEmpty() || strs.Length < 2)
                    {
                        continue;
                    }

                    SetPropertyValue(model, strs[0], strs[1]);
                }

                return model;
            }
        }

        public static ImageCacheModel ReadImageCacheInfo(string guid)
        {
            var file = new FileInfo(CodeApi.ImagePath + guid + CodeApi.ImageExtension);
            return !file.Exists ? null : ReadImageCacheInfo(file);
        }

        private static void SetPropertyValue(object obj, string propName, string propValue)
        {
            var type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                if (prop.Name == propName)
                {
                    prop.SetValue(obj, Convert.ChangeType(propValue, prop.PropertyType));
                }
            }
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
    }
}