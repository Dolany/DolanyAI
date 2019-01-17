﻿using Autofac;
using Dolany.Ai.MQ.Db;
using Dolany.Ai.Util;

namespace Dolany.Ai.MQ
{
    using System;
    using System.IO;
    using System.Text;

    using Newbe.Mahua;
    using MahuaEvents;
    using Newbe.Mahua.MahuaEvents;

    /// <summary>
    /// Ioc容器注册
    /// </summary>
    public class MahuaModule : IMahuaModule
    {
        public Module[] GetModules()
        {
            // 可以按照功能模块进行划分，此处可以改造为基于文件配置进行构造。实现模块化编程。
            return new Module[]
                       {
                           new PluginModule(),
                           new MahuaEventsModule(),
                           new DolanyModule()
                       };
        }

        /// <summary>
        /// 基本模块
        /// </summary>
        private class PluginModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                // 将实现类与接口的关系注入到Autofac的Ioc容器中。如果此处缺少注册将无法启动插件。
                // 注意！！！PluginInfo是插件运行必须注册的，其他内容则不是必要的！！！
                builder.RegisterType<PluginInfo>().As<IPluginInfo>();

                //注册在“设置中心”中注册菜单，若想订阅菜单点击事件，可以查看教程。http://www.newbe.pro/docs/mahua/2017/12/24/Newbe-Mahua-Navigations.html
                builder.RegisterType<MyMenuProvider>().As<IMahuaMenuProvider>();
            }
        }

        /// <summary>
        /// <see cref="IMahuaEvent"/> 事件处理模块
        /// </summary>
        private class MahuaEventsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                // 将需要监听的事件注册，若缺少此注册，则不会调用相关的实现类
                builder.RegisterType<GroupMsgReceive>().As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<PrivateMsgReceived>().As<IPrivateMessageReceivedMahuaEvent>();
                builder.RegisterType<InitializationMahuaEvent>().As<IInitializationMahuaEvent>();
            }
        }

        private class DolanyModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                RabbitMQService.Instance.StartReceive();
                base.Load(builder);
            }
        }

        public static class RuntimeLogger
        {
            private const string LogPath = "./RuntimeLog/";
            private static readonly object lockObj = new object();

            public static void Log(string log)
            {
                lock (lockObj)
                {
                    var steam = CheckFile();
                    var data = new UTF8Encoding().GetBytes($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}:{log}\r\n");
                    steam.Write(data, 0, data.Length);
                    //清空缓冲区、关闭流
                    steam.Flush();
                    steam.Close();
                }
            }

            public static void Log(Exception ex)
            {
                while (true)
                {
                    Log(ex.Message + "\r\n" + ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        continue;
                    }

                    break;
                }

                InfoSender.Send(AiInformation.Error, ex.Message + "\r\n" + ex.StackTrace);
            }

            private static FileStream CheckFile()
            {
                var dir = new DirectoryInfo(LogPath);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                var fi = new FileInfo(LogPath + DateTime.Now.ToString("yyyyMMdd") + ".log");
                return !fi.Exists ? fi.Create() : fi.Open(FileMode.Append, FileAccess.Write);
            }
        }
    }
}
