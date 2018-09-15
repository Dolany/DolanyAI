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
            var content = $@"<?xml version=""1.0"" encoding=""utf - 8""?>
                <msg serviceID = ""1"" brief = ""新消息"" templateID = """" action = ""新消息"" sourceMsgId = ""0"" url = """" flag = ""1"" adverSign = ""0"" multiMsgFlag = ""0"">
                     <item layout = ""0"">
                          <title color = ""#ff0000"" size = ""50""> 改成你自己的内容1 </title>
                       </item>
                       <item layout = ""0"">
                            <title color = ""#ff0000"" size = ""50""> 改成你自己的内容2 </title>
                         </item>
                     </msg>";
            var strs = content.Split(new[] { '\r', '\n' });
            content = string.Join("", strs);

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = content
            });
        }
    }
}