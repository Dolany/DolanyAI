using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AmandaSharp
{
    internal class Plugin
    {
        public string ID; //插件的唯一ID，内部标识
        public string Name; //插件的名称
        public string Author; //这是插件的作者
        public string Version; //插件的版本号
        public string Brief; //插件的简介&说明
        public string SDK; //插件的SDK版本号，唯一值，固定不变
        public string WindowsTitle; //这里写上需要拓展的窗口名称，用{子程序名=窗口名-载入方式}括起来，如果没有设置窗口则留空

        public string GetTextInfo()
        {
            string ret;
            ret = "pluginID=" + ID + ";\n";
            ret += "pluginName=" + Name + ";\n";
            ret += "pluginBrief=" + Brief + ";\n";
            ret += "pluginVersion=" + Version + ";\n";
            ret += "pluginSDK=" + SDK + ";\n";
            ret += "pluginAuthor=" + Author + ";\n";
            ret += "pluginWindowsTitle=" + WindowsTitle + ";\n";
            return ret;
        }
    };

    internal class API
    {
        public static string AuthCode = "";
        public const int MSG_CONTINUE = 0; //消息_继续执行
        public const int MSG_INTERCEPT = 1; //消息_拦截
        public const int ASK_CONSENT = 1; //请求_同意
        public const int ASK_REFUSE = 2; //请求_拒绝
        public const int ASK_NEGLECT = 3; //请求_忽略

        #region extern

        [DllImport("bin\\message.dll")]
        private static extern int Api_SendPraise(string QQ号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SendShake(string QQ号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_Getbkn(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_QuitGroup(string 群号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_QuitDiscussGroup(string 讨论组号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SetSignature(string 个性签名, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SetDiscussName(string 讨论组号, string 名称, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_BanGroup(string 群号, bool 是否全群禁言, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_RemoveGroup(string 群号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SetManager(string 群号, string QQ号, bool 是否设置为管理员, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_Ban(string 群号, string QQ, int 禁言时长, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SetAnony(string 群号, bool 是否允许匿名聊天, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_RemoveMember(string 群号, string QQ号, bool 是否不再接收加群申请, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetCookies(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetClientKey(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetLoginQQ(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern bool Api_GetPluginState(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetPath(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SendMsg(int 类型, string 群组, string QQ号, string 内容, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SetGroupAdd(string 群号, string QQ号, string Seq, int 操作方式, string 拒绝理由, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SetFriendAdd(string QQ号, int 操作方式, string 拒绝理由, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SendLog(string 类型, string 内容, int 字体颜色, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SetFriendName(string 好友QQ, string 备注名, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_DeleteFriend(string 好友QQ, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_AddGroup(string 群号, string 附加信息, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern void Api_AddFriend(string 目标QQ, string 附加信息, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_163Music(string 歌曲ID, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_QQMusic(string 歌曲ID, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_JsonMusic(string 歌曲ID, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_Error(string Code, string Str, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetGroupCard(string 群号, string QQ号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetNick(string QQ号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SetGroupCard(string 群号, string QQ号, string 新名片, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetPraiseNum(string QQ号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetFriendList(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetGroupList(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetGroupMemberList(string 群号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetQQInfo(string QQ号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetGroupInfo(string 群号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_Restart(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_UpdatePlugin(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_DeleteMsg(string 群号, string 消息ID, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_SetQQState(int 类型, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_InviteFriend(string 群号, string 好友QQ, string AuthCode);

        [DllImport("bin\\message.dll")]

        #endregion extern

        private static extern string Api_GetQQinfo_v2(string QQ号, string AuthCode);

        // 	Api_点赞
        public static int SendPraise(string QQ号)
        {
            return Api_SendPraise(QQ号, AuthCode);
        }

        // 	Api_抖动好友窗口
        public static int SendShake(string QQ号)
        {
            return Api_SendShake(QQ号, AuthCode);
        }

        // 	取bkn
        public static string Getbkn()
        {
            return Api_Getbkn(AuthCode);
        }

        // 	退出群
        public static int QuitGroup(string 群号) { return Api_QuitGroup(群号, AuthCode); }

        // 	退出讨论组
        public static int QuitDiscussGroup(string 讨论组号) { return Api_QuitDiscussGroup(讨论组号, AuthCode); }

        // 	修改个性签名
        public static int SetSignature(string 个性签名) { return Api_SetSignature(个性签名, AuthCode); }

        // 	修改讨论组名称
        public static int SetDiscussName(string 讨论组号, string 名称) { return Api_SetDiscussName(讨论组号, 名称, AuthCode); }

        // 	置全群禁言
        public static int BanGroup(string 群号, bool 是否全群禁言) { return Api_BanGroup(群号, 是否全群禁言, AuthCode); }

        // 	解散群
        public static int RemoveGroup(string 群号) { return Api_RemoveGroup(群号, AuthCode); }

        // 	置群管理
        public static int SetManager(string 群号, string QQ号, bool 是否设置为管理员) { return Api_SetManager(群号, QQ号, 是否设置为管理员, AuthCode); }

        // 	禁言
        public static int Ban(string 群号, string QQ, int 禁言时长) { return Api_Ban(群号, QQ, 禁言时长, AuthCode); }

        // 	置群匿名
        public static int SetAnony(string 群号, bool 是否允许匿名聊天) { return Api_SetAnony(群号, 是否允许匿名聊天, AuthCode); }

        // 	移除群成员
        public static int RemoveMember(string 群号, string QQ号, bool 是否不再接收加群申请) { return Api_RemoveMember(群号, QQ号, 是否不再接收加群申请, AuthCode); }

        // 	取Cookies
        public static string GetCookies() { return Api_GetCookies(AuthCode); }

        // 	取ClientKey
        public static string GetClientKey() { return Api_GetClientKey(AuthCode); }

        // 	取登录QQ
        public static string GetLoginQQ() { return Api_GetLoginQQ(AuthCode); }

        // 	取插件当前状态
        public static bool GetPluginState() { return Api_GetPluginState(AuthCode); }

        // 	取插件目录
        public static string GetPath() { return Api_GetPath(AuthCode); }

        // 	发送消息
        public static int SendMsg(int 类型, string 群组, string QQ号, string 内容) { return Api_SendMsg(类型, 群组, QQ号, 内容, AuthCode); }

        // 	置群添加请求
        public static int SetGroupAdd(string 群号, string QQ号, string Seq, int 操作方式, string 拒绝理由) { return Api_SetGroupAdd(群号, QQ号, Seq, 操作方式, 拒绝理由, AuthCode); }

        // 	置好友添加请求
        public static int SetFriendAdd(string QQ号, int 操作方式, string 拒绝理由) { return Api_SetFriendAdd(QQ号, 操作方式, 拒绝理由, AuthCode); }

        // 	输出日志
        public static int SendLog(string 类型, string 内容, int 字体颜色) { return Api_SendLog(类型, 内容, 字体颜色, AuthCode); }

        // 	修改好友备注
        public static int SetFriendName(string 好友QQ, string 备注名) { return Api_SetFriendName(好友QQ, 备注名, AuthCode); }

        // 	删除好友
        public static int DeleteFriend(string 好友QQ) { return Api_DeleteFriend(好友QQ, AuthCode); }

        // 	加群
        public static int AddGroup(string 群号, string 附加信息) { return Api_AddGroup(群号, 附加信息, AuthCode); }

        // 	加好友
        public static void AddFriend(string 目标QQ, string 附加信息) { Api_AddFriend(目标QQ, 附加信息, AuthCode); }

        // 	网易云点歌
        public static string _163Music(string 歌曲ID) { return Api_163Music(歌曲ID, AuthCode); }

        // 	QQ点歌
        public static string QQMusic(string 歌曲ID) { return Api_QQMusic(歌曲ID, AuthCode); }

        // 	新点歌
        public static string JsonMusic(string 歌曲ID) { return Api_JsonMusic(歌曲ID, AuthCode); }

        // 	置插件错误提示
        public static string Error(string Code, string Str) { return Api_Error(Code, Str, AuthCode); }

        // 	取群名片
        public static string GetGroupCard(string 群号, string QQ号) { return Api_GetGroupCard(群号, QQ号, AuthCode); }

        // 	取昵称
        public static string GetNick(string QQ号) { return Api_GetNick(QQ号, AuthCode); }

        // 	修改群名片
        public static int SetGroupCard(string 群号, string QQ号, string 新名片) { return Api_SetGroupCard(群号, QQ号, 新名片, AuthCode); }

        // 	取名片赞数量
        public static string GetPraiseNum(string QQ号) { return Api_GetPraiseNum(QQ号, AuthCode); }

        // 	取好友列表
        public static string GetFriendList() { return Api_GetFriendList(AuthCode); }

        // 	取群列表
        public static string GetGroupList() { return Api_GetGroupList(AuthCode); }

        // 	取群成员列表
        public static string GetGroupMemberList(string 群号) { return Api_GetGroupMemberList(群号, AuthCode); }

        // 	取QQ信息
        public static string GetQQInfo(string QQ号) { return Api_GetQQInfo(QQ号, AuthCode); }

        // 	取群信息
        public static string GetGroupInfo(string 群号) { return Api_GetGroupInfo(群号, AuthCode); }

        // 	重启机器人
        public static string Restart() { return Api_Restart(AuthCode); }

        // 	刷新插件列表
        public static string UpdataPlugin() { return Api_UpdatePlugin(AuthCode); }

        // 	撤回信息
        public static string DeleteMsg(string 群号, string 消息ID) { return Api_DeleteMsg(群号, 消息ID, AuthCode); }

        // 	置在线状态  1.我在线上 2.Q我吧 3.离开 4.忙碌 5.请勿打扰 6.隐身
        public static string SetQQState(int 类型) { return Api_SetQQState(类型, AuthCode); }

        // 	邀请好友入群
        public static string InviteFriend(string 群号, string 好友QQ) { return Api_InviteFriend(群号, 好友QQ, AuthCode); }

        // 	取QQ会员信息
        public static string GetQQinfo_v2(string QQ号) { return Api_GetQQinfo_v2(QQ号, AuthCode); }
    }
}