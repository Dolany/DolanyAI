using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Dolany.Ai.Common
{
    /// <summary>
    /// DI服务
    /// </summary>
    public class AutofacSvc
    {
        /// <summary>
        /// 容器
        /// </summary>
        public static IContainer Container;

        /// <summary>
        /// 类型解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// 类型解析
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        /// <summary>
        /// 按程序集批量注册服务
        /// </summary>
        /// <param name="assemblies"></param>
        public static void RegisterAutofac(List<Assembly> assemblies)
        {
            var builder = new ContainerBuilder();
            var baseType = typeof(IDependency);

            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract).AsSelf()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            Container = builder.Build();
        }

        /// <summary>
        /// 初始哈数据托管实例
        /// </summary>
        /// <param name="assemblies"></param>
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
            assembly ??= Assembly.GetAssembly(typeof(T));
            var list = assembly?.GetTypes().Where(type => type.IsSubclassOf(typeof(T)) && !type.IsAbstract).Where(type => type.FullName != null).Select(type =>
                Container.IsRegistered(type) ? Container.Resolve(type) as T : assembly.CreateInstance(type.FullName) as T);

            return list?.ToList();
        }

        public static List<T> LoadAllInstanceFromClass<T>(IEnumerable<Assembly> assemblies) where T : class
        {
            return assemblies.SelectMany(p => p.GetTypes().Where(type => type.IsSubclassOf(typeof(T)) && !type.IsAbstract))
                .Where(type => Container.IsRegistered(type)).Select(type => Resolve(type) as T).Where(d => d != null).ToList();
        }

        public static List<T> LoadAllInstanceFromInterface<T>(Assembly assembly = null) where T : class
        {
            assembly ??= Assembly.GetAssembly(typeof(T));
            var list = assembly?.GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Where(type => type.FullName != null)
                .Select(type => Container.IsRegistered(type) ? Container.Resolve(type) as T : assembly.CreateInstance(type.FullName) as T);

            return list?.ToList();
        }

        public static List<T> LoadAllInstanceFromInterface<T>(IEnumerable<Assembly> assemblies) where T : class
        {
            return assemblies.SelectMany(p => p.GetTypes().Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract))
                .Where(type => Container.IsRegistered(type)).Select(type => Resolve(type) as T).Where(d => d != null)
                .ToList();
        }
    }
}
