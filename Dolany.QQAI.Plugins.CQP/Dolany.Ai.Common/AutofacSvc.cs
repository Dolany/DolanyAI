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
            var baseType = typeof(IDataMgr);
            var datamgrs = assemblies.SelectMany(p => p.GetTypes().Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract))
                .Where(type => Container.IsRegistered(type)).Select(type => Resolve(type) as IDataMgr).Where(d => d != null).ToList();

            foreach (var datamgr in datamgrs)
            {
                datamgr.RefreshData();
                Container.Resolve<DataRefreshSvc>().Register(datamgr);
            }
        }
    }
}
