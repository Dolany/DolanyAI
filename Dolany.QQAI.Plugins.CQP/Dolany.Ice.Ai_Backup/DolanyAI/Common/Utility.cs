using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class Utility
    {
        private static Dictionary<Type, Object> SinglonMap;

        public static long DeveloperNumber
        {
            get
            {
                return 1458978159;
            }
        }

        public static void SendMsgToDeveloper(string msg)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                api.SendPrivateMessage(DeveloperNumber.ToString(), msg);
            }
        }

        public static void SendMsgToDeveloper(Exception ex)
        {
            SendMsgToDeveloper(ex.Message);
            SendMsgToDeveloper(ex.StackTrace);
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
            string[] strs = timeStr.Split(new char[] { ':', '：' });
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

        //public static void SetConfig(string name, string value)
        //{
        //    var query = DbMgr.Query<ConfigEntity>(c => c.Name == name);
        //    if (query.IsNullOrEmpty())
        //    {
        //        DbMgr.Insert(new ConfigEntity
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            Name = name,
        //            Content = value
        //        });
        //    }
        //    else
        //    {
        //        var config = query.First();
        //        config.Content = value;
        //        DbMgr.Update(config);
        //    }
        //}

        //public static string GetConfig(string name)
        //{
        //    var query = DbMgr.Query<ConfigEntity>(c => c.Name == name);
        //    if (query.IsNullOrEmpty())
        //    {
        //        return string.Empty;
        //    }

        //    return query.First().Content;
        //}

        public static T Instance<T>() where T : class, new()
        {
            if (SinglonMap == null)
            {
                SinglonMap = new Dictionary<Type, object>();
            }

            Type type = typeof(T);
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
            Type type = obj.GetType();
            T copyT = new T();
            foreach (var prop in type.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(copyT, prop.GetValue(obj));
                }
            }

            return copyT;
        }

        //public static GroupMemberInfo GetMemberInfo(GroupMsgDTO MsgDTO)
        //{
        //    return Instance<GroupMemberInfoCacher>().GetMemberInfo(MsgDTO);
        //}
    }
}