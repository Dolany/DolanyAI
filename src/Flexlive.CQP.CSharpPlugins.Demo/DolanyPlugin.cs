using Flexlive.CQP.Framework;
using System;
using System.Threading;
using AILib;
using System.Collections.Generic;
using System.Xml.Linq;
using AILib.Entities;

namespace Flexlive.CQP.CSharpPlugins.Demo
{
    public class DolanyPlugin : CQAppAbstract
    {
        /// <summary>
        /// 群号列表
        /// </summary>
        public long[] GroupList
        {
            get
            {
                List<long> list = new List<long>();

                var configs = DbMgr.Query<GroupConfigEntity>();
                if (configs == null)
                {
                    return list.ToArray();
                }
                foreach (var config in configs)
                {
                    list.Add(long.Parse(config.Content));
                }

                RuntimeLogger.Log("加载群号列表");
                return list.ToArray();
            }
        }

        /// <summary>
        /// 应用初始化，用来初始化应用的基本信息。
        /// </summary>
        public override void Initialize()
        {
            // 此方法用来初始化插件名称、版本、作者、描述等信息，
            // 不要在此添加其它初始化代码，插件初始化请写在Startup方法中。

            this.Name = "Dolany插件";
            this.Version = new Version("1.0.0.0");
            this.Author = "Dolany";
            this.Description = "Dolany制作的CQA插件，用于实验目的。";
        }

        /// <summary>
        /// 应用启动，完成插件线程、全局变量等自身运行所必须的初始化工作。
        /// </summary>
        public override void Startup()
        {
            //完成插件线程、全局变量等自身运行所必须的初始化工作。
            try
            {
                RuntimeLogger.Log("start up");
                DbMgr.InitXmls();

                // 获取可用AI列表
                var AIs = AIMgr.AllAIs;
                if (AIs.Count == 0)
                {
                    Common.SendMsgToDeveloper("加载ai列表失败");
                }

                RuntimeLogger.Log("加载所有可用AI");
                List<string> l = new List<string>();
                foreach (var ai in AIs)
                {
                    l.Add(ai.Name);
                }
                AIMgr.StartAIs(l.ToArray(), new AIConfigDTO()
                {
                    AimGroups = GroupList
                });

                var allais = AIMgr.AllAIs;
                string msg = $@"成功加载{allais.Count}个ai";
                foreach (var ai in allais)
                {
                    msg += '\r' + ai.Name + ":" + ai.Description;
                }

                Common.SendMsgToDeveloper(msg);
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        /// <summary>
        /// 打开设置窗口。
        /// </summary>
        public override void OpenSettingForm()
        {
            // 打开设置窗口的相关代码。
            FormSettings frm = new FormSettings();
            frm.ShowDialog();
        }

        /// <summary>
        /// Type=21 私聊消息。
        /// </summary>
        /// <param name="subType">子类型，11/来自好友 1/来自在线状态 2/来自群 3/来自讨论组。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public override void PrivateMessage(int subType, int sendTime, long fromQQ, string msg, int font)
        {
            // 处理私聊消息。
            RuntimeLogger.Log($"receive private message: fromQQ:{fromQQ} msg:{msg} time:{DateTime.Now}");
            try
            {
                AIMgr.OnPrivateMsgReceived(new PrivateMsgDTO()
                {
                    subType = subType,
                    sendTime = sendTime,
                    fromQQ = fromQQ,
                    msg = msg,
                    font = font
                });
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        /// <summary>
        /// Type=2 群消息。
        /// </summary>
        /// <param name = "subType" > 子类型，目前固定为1。</param>
        /// <param name = "sendTime" > 发送时间(时间戳)。</param>
        /// <param name = "fromGroup" > 来源群号。</param>
        /// <param name = "fromQQ" > 来源QQ。</param>
        /// <param name = "fromAnonymous" > 来源匿名者。</param>
        /// <param name = "msg" > 消息内容。</param>
        /// <param name = "font" > 字体。</param>
        public override void GroupMessage(int subType, int sendTime, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font)
        {
            RuntimeLogger.Log($"receive group message: fromGroup:{fromGroup} fromQQ:{fromQQ} msg:{msg} time:{DateTime.Now}");
            // 处理群消息。
            try
            {
                AIMgr.OnGroupMsgReceived(new GroupMsgDTO()
                {
                    subType = subType,
                    sendTime = sendTime,
                    fromGroup = fromGroup,
                    fromQQ = fromQQ,
                    fromAnonymous = fromAnonymous,
                    msg = msg,
                    fullMsg = msg,
                    font = font
                });
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }
    }
}