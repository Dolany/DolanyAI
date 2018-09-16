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
            SyntaxChecker = "Empty",
            Tag = "xml卡片功能"
        )]
        public void Xml(GroupMsgDTO MsgDTO, object[] param)
        {
            var content = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
                <msg serviceID = ""1"">
                    <item layout = ""2"">
                        <title>今日运势</title>
                        <summary>你今天的运势是：26%
||||||||||||||||||||||||||</summary>
                        <picture cover = ""https://s15.postimg.cc/41wr57fh7/image.jpg"" />
                    </item>
                </msg>";
            var strs = content.Split('\r', '\n');
            content = string.Join("", strs.Select(p => p.Trim()));

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = content
            });
        }
    }
}