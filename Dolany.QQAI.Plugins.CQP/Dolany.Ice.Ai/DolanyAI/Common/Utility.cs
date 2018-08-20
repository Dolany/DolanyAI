using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.IO;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class Utility
    {
        private static Dictionary<Type, Object> SinglonMap;
        private static string AuthCode;

        public static long DeveloperNumber
        {
            get
            {
                return 1458978159;
            }
        }

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
            if (objs == null || objs.Count() == 0)
            {
                return true;
            }

            return false;
        }

        public static string ToDateString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static (int hour, int minute)? GenTimeFromStr(string timeStr)
        {
            var strs = timeStr.Split(new char[] { ':', '：' });
            if (strs == null || strs.Length != 2)
            {
                return null;
            }

            int hour;
            if (!int.TryParse(strs[0], out hour))
            {
                return null;
            }

            int minute;
            if (!int.TryParse(strs[1], out minute))
            {
                return null;
            }

            return (hour, minute);
        }

        public static void SetConfig(string name, string value)
        {
            using (AIDatabase db = new AIDatabase())
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

        public static string GetConfig(string name)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.AIConfig.Where(p => p.Key == name);
                if (query.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                return query.First().Value;
            }
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
            return Instance<GroupMemberInfoCacher>().GetMemberInfo(MsgDTO);
        }

        public static string GetAuthCode()
        {
            if (string.IsNullOrEmpty(AuthCode))
            {
                using (AmandaLogDatabase db = new AmandaLogDatabase())
                {
                    var log = db.日志.Where(p => p.内容.Contains("Dolany AI(Dolany.Ice.Ai)")).OrderByDescending(p => p.时间).First();
                    var strs = log.内容.Split(new string[] { "调用内存：" }, StringSplitOptions.RemoveEmptyEntries);
                    AuthCode = strs[1];
                }
            }

            return AuthCode;
        }

        public static ImageCacheModel ReadImageCacheInfo(FileInfo file)
        {
            using (StreamReader reader = new StreamReader(file.FullName))
            {
                var model = new ImageCacheModel();

                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    var strs = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
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
            if (!file.Exists)
            {
                return null;
            }

            return ReadImageCacheInfo(file);
        }

        public static void SetPropertyValue(Object obj, string propName, string propValue)
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
            foreach (var c in name)
            {
                if (IsAsciiChar(c))
                {
                    result += c;
                    continue;
                }

                result += "%" + BitConverter.ToString(Encoding.UTF8.GetBytes(new char[] { c })).Replace("-", "%");
            }

            return result;
        }

        public static bool IsAsciiChar(char c)
        {
            return c >= 0x20 && c <= 0x7e;
        }
    }
}