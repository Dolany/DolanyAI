using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Dolany.Ai.Common
{
    public class AutofacSvc
    {
        public static IContainer Container;

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public static void RegisterAutofac(List<Assembly> assemblies)
        {
            var builder = new ContainerBuilder();
            var baseType = typeof(IDependency);

            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract).AsSelf()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            Container = builder.Build();
        }

        public static void RegisterDataRefresher(IEnumerable<Assembly> assemblies)
        {
            var datamgrs = LoadAllInstanceFromInterface<IDataMgr>(assemblies);

            foreach (var datamgr in datamgrs)
            {
                datamgr.RefreshData();
                Container.Resolve<DataRefreshSvc>().Register(datamgr);
            }
        }

        public static List<T> LoadAllInstanceFromClass<T>(Assembly assembly = null) where T : class
        {
            assembly = assembly == null ? Assembly.GetAssembly(typeof(T)) : assembly;
            var list = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T)) && !type.IsAbstract).Where(type => type.FullName != null).Select(type =>
                Container.IsRegistered(type) ? Container.Resolve(type) as T : assembly.CreateInstance(type.FullName) as T);

            return list.ToList();
        }

        public static List<T> LoadAllInstanceFromClass<T>(IEnumerable<Assembly> assemblies) where T : class
        {
            var baseType = typeof(T);
            return assemblies.SelectMany(p => p.GetTypes().Where(type => type.IsSubclassOf(typeof(T)) && !type.IsAbstract))
                .Where(type => Container.IsRegistered(type)).Select(type => Resolve(type) as T).Where(d => d != null).ToList();
        }

        public static List<T> LoadAllInstanceFromInterface<T>(Assembly assembly = null) where T : class
        {
            assembly = assembly == null ? Assembly.GetAssembly(typeof(T)) : assembly;
            var list = assembly.GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Where(type => type.FullName != null)
                .Select(type => Container.IsRegistered(type) ? Container.Resolve(type) as T : assembly.CreateInstance(type.FullName) as T);

            return list.ToList();
        }

        public static List<T> LoadAllInstanceFromInterface<T>(IEnumerable<Assembly> assemblies) where T : class
        {
            var baseType = typeof(T);
            return assemblies.SelectMany(p => p.GetTypes().Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract))
                .Where(type => Container.IsRegistered(type)).Select(type => Resolve(type) as T).Where(d => d != null)
                .ToList();
        }
    }
}
