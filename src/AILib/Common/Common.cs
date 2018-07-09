using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework;
using AILib.Entities;
using System.ComponentModel.Composition;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;

namespace AILib
{
    public static class Common
    {
        public static long DeveloperNumber
        {
            get
            {
                return 1458978159;
            }
        }

        public static void SendMsgToDeveloper(string msg)
        {
            CQ.SendPrivateMessage(DeveloperNumber, msg);
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

        public static void SetConfig(string name, string value)
        {
            var query = DbMgr.Query<ConfigEntity>(c => c.Name == name);
            if (query.IsNullOrEmpty())
            {
                DbMgr.Insert(new ConfigEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Content = value
                });
            }
            else
            {
                var config = query.First();
                config.Content = value;
                DbMgr.Update(config);
            }
        }

        public static string GetConfig(string name)
        {
            var query = DbMgr.Query<ConfigEntity>(c => c.Name == name);
            if (query.IsNullOrEmpty())
            {
                return string.Empty;
            }

            return query.First().Content;
        }

        public static T ComposePartsSelf<T>(this T obj) where T : class
        {
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            catalog.Catalogs.Add(new DirectoryCatalog("."));

            var _container = new CompositionContainer(catalog);

            _container.ComposeParts(obj);

            return obj;
        }

        public static T ComposePartsSelf<T>(this T obj, Assembly assembly) where T : class
        {
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            catalog.Catalogs.Add(new DirectoryCatalog("."));

            var _container = new CompositionContainer(catalog);

            _container.ComposeParts(obj);

            return obj;
        }
    }
}