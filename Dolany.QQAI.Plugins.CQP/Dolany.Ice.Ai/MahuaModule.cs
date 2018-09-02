using System.Collections.Generic;
using Autofac;
using Newbe.Mahua;

using Dolany.Ice.Ai.DolanyAI;
using Newbe.Mahua.MahuaEvents;
using Dolany.Ice.Ai.MahuaEvents;
using System.Linq;

namespace Dolany.Ice.Ai
{
    /// <inheritdoc />
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
                new AIModule()
            };
        }

        /// <inheritdoc />
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
                builder.RegisterType<PluginInfo>()
                    .As<IPluginInfo>();

                //注册在“设置中心”中注册菜单，若想订阅菜单点击事件，可以查看教程。http://www.newbe.pro/docs/mahua/2017/12/24/Newbe-Mahua-Navigations.html
                builder.RegisterType<MyMenuProvider>()
                    .As<IMahuaMenuProvider>();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// <see cref="T:Newbe.Mahua.IMahuaEvent" /> 事件处理模块
        /// </summary>
        private class MahuaEventsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                // 将需要监听的事件注册，若缺少此注册，则不会调用相关的实现类

                builder.RegisterType<AIGroupMsgReceived>()
                    .As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<AIPrivateMsgReceived>()
                    .As<IPrivateMessageFromFriendReceivedMahuaEvent>();
            }
        }

        private class AIModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                RuntimeLogger.Log("start up");
                DbMgr.InitXmls();
                RuntimeLogger.Log("加载所有可用AI");

                AIMgr.Instance.StartAIs();

                var allais = AIMgr.Instance.AIList;
                var keyValuePairs = allais as KeyValuePair<AIBase, AIAttribute>[] ?? allais.ToArray();
                var msg = $"成功加载{keyValuePairs.Length}个ai \r\n";
                var builder1 = new System.Text.StringBuilder();
                builder1.Append(msg);
                foreach (var ai in keyValuePairs)
                {
                    builder1.Append(ai.Value.Name + " ");
                }
                msg = builder1.ToString();
                RuntimeLogger.Log(msg);
            }
        }
    }
}