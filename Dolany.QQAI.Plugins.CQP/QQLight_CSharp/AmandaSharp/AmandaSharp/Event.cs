using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System;

namespace AmandaSharp
{
    public class Event
    {
        private static string Path;
        private static Profiles profiles;

        public Event()
        {
        }

        [DllExport(ExportName = nameof(Information), CallingConvention = CallingConvention.StdCall)]
        public static string Information(string AuthCode)
        {
            API.AuthCode = AuthCode;
            var p = new Plugin
            {
                ID = "dolany.iceai.AmandaSharp",//插件的唯一ID，内部标识
                Name = "DolanyAi", //插件的名称
                Author = "Dolany", //这是插件的作者
                Version = "1.0.0", // 插件的版本号
                Brief = "Dolany Ai",  // 插件的简介&说明
                SDK = "3",// 插件的SDK版本号，唯一值，固定不变
                WindowsTitle = "{_TestMenu1=设置-真}"
            };
            // 这里写上需要拓展的窗口名称，用{子程序名=窗口名-载入方式}括起来，如果没有设置窗口则留空
            //' 载入方式的真或者假代表着此窗口是否以对话框方式加载，如果不加此声明则默认为真

            return p.GetTextInfo();
        }

        //初始化插件，插件加载时会调用此事件
        [DllExport(ExportName = nameof(Event_Initialization), CallingConvention = CallingConvention.StdCall)]
        public static int Event_Initialization()
        {
            Path = API.GetPath();
            //框架为插件所创建的一个目录，希望作者们把当前插件的所有数据都写入到此目录下面，以免跟其他插件混淆
            return 0;
        }

        //插件被启用事件
        [DllExport(ExportName = nameof(Event_pluginStart), CallingConvention = CallingConvention.StdCall)]
        public static int Event_pluginStart()
        {
            /*
            插件被开启时会执行此事件
            机器人登录时如果插件开关是开启状态，此事件也会发生一次
            */
            API.SendLog(nameof(Path), Path, 0x00ff00);
            profiles = new Profiles();
            API.SendLog("Info", "插件开启成功", 0xff0000);
            return 0;
        }

        //插件被关闭事件
        [DllExport(ExportName = nameof(Event_pluginStop), CallingConvention = CallingConvention.StdCall)]
        public static int Event_pluginStop()
        {
            //一般用来销毁各种线程或者资源
            return 0;
        }

        //获取最新信息(好友/群/群临时/讨论组/讨论组临时消息/QQ临时消息)事件
        [DllExport(ExportName = nameof(Event_GetNewMsg), CallingConvention = CallingConvention.StdCall)]
        public static int Event_GetNewMsg(int type, string GroupID, string FromQQ, string Msg, string MsgID)
        {
            //string s = API.GetGroupMemberList(GroupID);
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        //QQ财付通转账事件
        [DllExport(ExportName = nameof(Event_GetQQWalletData), CallingConvention = CallingConvention.StdCall)]
        public static int Event_GetQQWalletData(int type, string GroupID, string FromQQ, string Sum, string Msg, string Order)
        {
            /*
            type 1.好友转账 2.群临时转账 3.讨论组临时转账
            GroupID 类型1.此参数为空 2.群号 3.讨论组号
            FromQQ 来源QQ
            Sum 金额
            Msg 备注
            Order 订单号
            */
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        //管理员变动事件
        [DllExport(ExportName = nameof(Event_AdminChange), CallingConvention = CallingConvention.StdCall)]
        public static int Event_AdminChange(int type, string GroupID, string FromQQ)
        {
            /*
            type 1.xx被添加管理 2.xx被解除管理
            GroupID 群号
            FromQQ 来源QQ
            */
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        //群成员增加事件
        [DllExport(ExportName = nameof(Event_GroupMemberIncrease), CallingConvention = CallingConvention.StdCall)]
        public static int Event_GroupMemberIncrease(int type, string GroupID, string JQQ, string OQQ)
        {
            /*
            type 1.主动入群  2.被xxx邀请入群
            GroupID 群号
            JQQ 加入的QQ
            OQQ 操作者QQ 类型为1.管理员 2.邀请人
            */
            API.SendMsg(2, GroupID, JQQ, "[QQ:at=" + JQQ + "]");
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        //群成员减少事件
        [DllExport(ExportName = nameof(Event_GroupMemberDecrease), CallingConvention = CallingConvention.StdCall)]
        public static int Event_GroupMemberDecrease(int type, string GroupID, string EQQ, string OQQ)
        {
            /*
            type 1.主动退群  2.被xxx踢出群
            GroupID 群号
            EQQ 退群的QQ
            OQQ 操作者QQ 类型为1时参数为空
            */
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        //群添加事件
        [DllExport(ExportName = nameof(Event_AddGroup), CallingConvention = CallingConvention.StdCall)]
        public static int Event_AddGroup(int type, string GroupID, string QQ, string IQQ, string additional, string Seq)
        {
            /*
            type 1.主动加群  2.被邀请进群 3.机器人被邀请入群
            GroupID 群号
            QQ QQ号
            IQQ 邀请者QQ 类型为1时参数为空
            additional 加群者的附加信息，类型为2，3时参数为空
            Seq 群添加事件产生的Seq标识
            */
            //Api_SetGroupAdd(GroupID,QQ,Seq,ASK_CONSENT,"");//同意入群
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        //被添加好友事件
        [DllExport(ExportName = nameof(Event_AddFrinend), CallingConvention = CallingConvention.StdCall)]
        public static int Event_AddFrinend(string QQ, string Msg)
        {
            /*
            QQ QQ号
            Msg 好友添加理由
            */
            API.SetFriendAdd(QQ, API.ASK_CONSENT, "");
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        //成为了好友事件
        [DllExport(ExportName = nameof(Event_BecomeFriends), CallingConvention = CallingConvention.StdCall)]
        public static int Event_BecomeFriends(string QQ)
        {
            /*
            QQ 好友QQ号
            */
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        //Cookies更新时会触发此事件
        [DllExport(ExportName = nameof(Event_UpdataCookies), CallingConvention = CallingConvention.StdCall)]
        public static int Event_UpdataCookies()
        {
            return API.MSG_CONTINUE;// 返回0 下个插件继续处理该事件，返回1 拦截此事件不让其他插件执行
        }

        [DllExport(ExportName = nameof(_TestMenu1), CallingConvention = CallingConvention.StdCall)]
        public static int _TestMenu1()
        {
            //MessageBox(0,TEXT("嘿，程序猿！"),TEXT("Msg:"),64);
            //new Setting().ShowDialog();

            using (var form = new SettingForm(profiles))
            {
                var dr = form.ShowDialog();
                //API.Api_SendLog("DialogResult: ", dr + "", 0xcccccc);
                return 0;
            }
        }
    }
}