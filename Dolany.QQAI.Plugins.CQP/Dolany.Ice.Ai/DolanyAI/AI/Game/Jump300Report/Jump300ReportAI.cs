using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
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

        [GroupEnterCommand(
            Command = "战绩查询",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询300英雄战绩信息",
            Syntax = " [角色名]",
            Tag = "战绩查询功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void JumpReport(GroupMsgDTO MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "查询中，请稍候"
            });
            var jr = new JumpReportRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        public void ReportCallBack(GroupMsgDTO MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = Report
            });
        }
    }
}