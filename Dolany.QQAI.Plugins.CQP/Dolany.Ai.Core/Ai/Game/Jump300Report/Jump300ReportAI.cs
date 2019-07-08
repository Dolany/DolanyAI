using System.Threading.Tasks;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Game.Jump300Report
{
    [AI(Name = "300英雄战绩查询",
        Description = "AI for 300 heros report.",
        Enable = false,
        PriorityLevel = 10)]
    public class Jump300ReportAI : AIBase
    {
        public override bool NeedManualOpeon { get; set; } = true;

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
