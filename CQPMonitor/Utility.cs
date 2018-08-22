using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;

namespace CQPMonitor
{
    public static class Utility
    {
        public static T ComposePartsSelf<T>(this T obj, Assembly assembly) where T : class
        {
            var catalog = new AggregateCatalog();

            using (var assemblyCatalog = new AssemblyCatalog(assembly))
            {
                catalog.Catalogs.Add(assemblyCatalog);
            }
            using (var directoryCatalog = new DirectoryCatalog("."))
            {
                catalog.Catalogs.Add(directoryCatalog);
            }

            using (var _container = new CompositionContainer(catalog))
            {
                _container.ComposeParts(obj);

                return obj;
            }
        }

        public static void SetConfig(string name, string value)
        {
            Dolany.Ice.Ai.DolanyAI.Utility.SetConfig(name, value);
        }

        public static string GetConfig(string name)
        {
            return Dolany.Ice.Ai.DolanyAI.Utility.GetConfig(name);
        }

        public static void SendMsgToDeveloper(string msg)
        {
            Dolany.Ice.Ai.DolanyAI.Utility.SendMsgToDeveloper(msg);
        }

        public static void SendMsgToDeveloper(Exception ex)
        {
            Dolany.Ice.Ai.DolanyAI.Utility.SendMsgToDeveloper(ex);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            if (objs == null || objs.Count() == 0)
            {
                return true;
            }

            return false;
        }
    }
}