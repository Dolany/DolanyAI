using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.IWS2000.Ai.DolanyAI
{
    [AI(
        Name = nameof(XmlCardAi),
        Description = "AI for XmlCard test.",
        IsAvailable = true,
        PriorityLevel = 10
    )]
    public class XmlCardAi : AIBase
    {
        private Dictionary<int, string> ConfigDic = Utility.LoadFortuneImagesConfig();

        public XmlCardAi()
        {
            RuntimeLogger.Log("XmlCardAi started.");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "xml",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "xml测试",
            Syntax = "",
            SyntaxChecker = "Long",
            Tag = "xml卡片功能"
        )]
        public void Xml(GroupMsgDTO MsgDTO, object[] param)
        {
            var fortune = (long)param[0];

            var content = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
                <msg serviceID = ""1"">
                    <item layout=""2"">
                        <title>今日运势</title>
                        <summary>你今天的运势是
{fortune}%</summary>
                        <picture cover = ""{ConfigDic[AlignFortune(fortune)]}"" />
                    </item>
                    <source name=""冰冰认证消息"" icon=""https://qzs.qq.com/ac/qzone_v5/client/auth_icon.png"" action="""" appid="" -1"" />
                </msg>";
            //var strs = content.Split('\r', '\n');
            //content = string.Join("", strs.Select(p => p.Trim()));

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = content
            });
        }

        private int AlignFortune(long fortune)
        {
            if(fortune >= 100)
            {
                return 100;
            }
            else if(fortune > 90)
            {
                return 90;
            }
            else if (fortune > 80)
            {
                return 80;
            }
            else if (fortune > 70)
            {
                return 70;
            }
            else if (fortune > 60)
            {
                return 60;
            }
            else if (fortune > 50)
            {
                return 50;
            }
            else if (fortune > 40)
            {
                return 40;
            }
            else if (fortune > 30)
            {
                return 30;
            }
            else if (fortune > 20)
            {
                return 20;
            }
            else if (fortune > 10)
            {
                return 10;
            }
            return 0;
        }
    }
}