using System.Threading.Tasks;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Game.Jump300Report
{
    public class Jump300ReportAI : AIBase
    {
        public override string AIName { get; set; } = "300英雄战绩查询";

        public override string Description { get; set; } = "AI for 300 heros report.";

        public override int PriorityLevel { get; set; } = 10;

        public override bool NeedManualOpeon { get; } = true;

        public override bool Enable { get; } = false;

        [EnterCommand(ID = "Jump300ReportAI_JumpReport",
            Command = "战绩查询",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询300英雄战绩信息",
            Syntax = " [角色名]",
            Tag = "游戏功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool JumpReport(MsgInformationEx MsgDTO, object[] param)
        {
            MsgSender.PushMsg(MsgDTO, "查询中，请稍候");
            var jr = new JumpReportRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
            return true;
        }

        private static void ReportCallBack(MsgInformationEx MsgDTO, string Report)
        {
            MsgSender.PushMsg(MsgDTO, Report);
        }
    }
}
