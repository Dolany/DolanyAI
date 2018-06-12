using Flexlive.CQP.Framework;
using System;
using System.Threading;
using AILib;
using System.Collections.Generic;
using System.Xml.Linq;
using AILib.Entities;

namespace Flexlive.CQP.CSharpPlugins.Demo
{
    /// <summary>
    /// 酷Q C#版插件Demo
    /// </summary>
    public class MyPlugin : CQAppAbstract
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
                if(configs == null)
                {
                    return list.ToArray();
                }
                foreach(var config in configs)
                {
                    list.Add(long.Parse(config.Content));
                }

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
                // 初始化xml
                DbMgr.InitXml();

                // 获取可用AI列表
                var AIs = AIMgr.AllAIs;
                if (AIs.Count == 0)
                {
                    Common.SendMsgToDeveloper("加载ai列表失败");
                }
                else
                {
                    Common.SendMsgToDeveloper($@"成功加载{AIs.Count}个ai");
                }

                // 加载所有可用AI
                List<string> l = new List<string>();
                foreach (var ai in AIs)
                {
                    l.Add(ai.Name);
                }
                AIMgr.StartAIs(l.ToArray(), new AIConfigDTO()
                {
                    AimGroups = GroupList
                });
            }
            catch(Exception ex)
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
                    font = font
                });
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        /// <summary>
        /// Type=4 讨论组消息。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromDiscuss">来源讨论组。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        //public override void DiscussMessage(int subType, int sendTime, long fromDiscuss, long fromQQ, string msg, int font)
        //{
        //    // 处理讨论组消息。
        //    CQ.SendDiscussMessage(fromDiscuss, String.Format("[{0}]{1}你发的讨论组消息是：{2}", CQ.ProxyType, CQ.CQCode_At(fromQQ), msg));
        //}

        /// <summary>
        /// Type=11 群文件上传事件。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="file">上传文件信息。</param>
        //public override void GroupUpload(int subType, int sendTime, long fromGroup, long fromQQ, string file)
        //{
        //    // 处理群文件上传事件。
        //    CQ.SendGroupMessage(fromGroup, String.Format("[{0}]{1}你上传了一个文件：{2}", CQ.ProxyType, CQ.CQCode_At(fromQQ), file));
        //}

        /// <summary>
        /// Type=101 群事件-管理员变动。
        /// </summary>
        /// <param name="subType">子类型，1/被取消管理员 2/被设置管理员。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        //public override void GroupAdmin(int subType, int sendTime, long fromGroup, long beingOperateQQ)
        //{
        //    // 处理群事件-管理员变动。
        //    CQ.SendGroupMessage(fromGroup, String.Format("[{0}]{2}({1})被{3}管理员权限。", CQ.ProxyType, beingOperateQQ, CQ.GetQQName(beingOperateQQ), subType == 1 ? "取消了" : "设置为"));
        //}

        /// <summary>
        /// Type=102 群事件-群成员减少。
        /// </summary>
        /// <param name="subType">子类型，1/群员离开 2/群员被踢 3/自己(即登录号)被踢。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        //public override void GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        //{
        //    // 处理群事件-群成员减少。
        //    CQ.SendGroupMessage(fromGroup, String.Format("[{0}]群员{2}({1}){3}", CQ.ProxyType, beingOperateQQ, CQ.GetQQName(beingOperateQQ), subType == 1 ? "退群。" : String.Format("被{0}({1})踢除。", CQ.GetQQName(fromQQ), fromQQ)));
        //}

        /// <summary>
        /// Type=103 群事件-群成员增加。
        /// </summary>
        /// <param name="subType">子类型，1/管理员已同意 2/管理员邀请。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        //public override void GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        //{
        //    // 处理群事件-群成员增加。
        //    CQ.SendGroupMessage(fromGroup, String.Format("[{0}]群里来了新人{2}({1})，管理员{3}({4}){5}", CQ.ProxyType, beingOperateQQ, CQ.GetQQName(beingOperateQQ), CQ.GetQQName(fromQQ), fromQQ, subType == 1 ? "同意。" : "邀请。"));
        //}

        /// <summary>
        /// Type=201 好友事件-好友已添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        //public override void FriendAdded(int subType, int sendTime, long fromQQ)
        //{
        //    // 处理好友事件-好友已添加。
        //    CQ.SendPrivateMessage(fromQQ, String.Format("[{0}]你好，我的朋友！", CQ.ProxyType));
        //}

        /// <summary>
        /// Type=301 请求-好友添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">附言。</param>
        /// <param name="responseFlag">反馈标识(处理请求用)。</param>
        //public override void RequestAddFriend(int subType, int sendTime, long fromQQ, string msg, string responseFlag)
        //{
        //    // 处理请求-好友添加。
        //    CQ.SetFriendAddRequest(responseFlag, CQReactType.Allow, "新来的朋友");
        //}

        /// <summary>
        /// Type=302 请求-群添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">附言。</param>
        /// <param name="responseFlag">反馈标识(处理请求用)。</param>
        //public override void RequestAddGroup(int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        //{
        //    // 处理请求-群添加。
        //    CQ.SetGroupAddRequest(responseFlag, CQRequestType.GroupAdd, CQReactType.Allow, "新群友");
        //}
    }
}
