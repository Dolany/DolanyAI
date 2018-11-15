using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.Reflection;
using System.ComponentModel.Composition.Hosting;

namespace AIMonitor
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

        public static string GetConfig(string name)
        {
            return GetConfig(name);
        }

        public static void SendMsgToDeveloper(string msg)
        {
            SendMsgToDeveloper(msg);
        }

        public static void SendMsgToDeveloper(Exception ex)
        {
            SendMsgToDeveloper(ex);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            return objs == null || !objs.Any();
        }
    }
}