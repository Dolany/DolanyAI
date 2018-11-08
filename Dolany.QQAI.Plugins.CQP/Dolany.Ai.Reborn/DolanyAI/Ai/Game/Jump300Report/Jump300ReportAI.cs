using System.Threading.Tasks;
using Dolany.Ai.Reborn.DolanyAI.Base;
using Dolany.Ai.Reborn.DolanyAI.Cache;
using Dolany.Ai.Reborn.DolanyAI.DTO;

namespace Dolany.Ai.Reborn.DolanyAI.Ai.Game.Jump300Report
{
    [AI(
        Name = nameof(Jump300ReportAI),
        Description = "AI for 300 heros report.",
        IsAvailable = true,
        PriorityLevel = 10
    )]
    public class Jump300ReportAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "战绩查询",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询300英雄战绩信息",
            Syntax = " [角色名]",
            Tag = "战绩查询功能",
            SyntaxChecker = "Word",
            IsPrivateAvailabe = true
        )]
        public void JumpReport(ReceivedMsgDTO MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(MsgDTO, "查询中，请稍候");
            var jr = new JumpReportRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        private static void ReportCallBack(ReceivedMsgDTO MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(MsgDTO, Report);
        }
    }
}
