using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Sys
{
    public class MonitorAI : AIBase
    {
        public override string AIName { get; set; } = "监视器";

        public override string Description { get; set; } = "AI for Monitoring and managing Ais status.";

        public override int PriorityLevel { get; set; } = 100;

        [EnterCommand(ID = "MonitorAI_SelfExcharge",
            Command = "自助充值",
            Description = "使用钻石自助为本群充值指定天数的机器人使用时间(1天=10钻石)",
            Syntax = "[天数]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = false)]
        public bool SelfExcharge(MsgInformationEx MsgDTO, object[] param)
        {
            var days = (int) (long) param[0];
            if (days <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "天数错误，请重新输入命令！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var diamondNeed = days * 10;
            if (osPerson.Diamonds < diamondNeed)
            {
                MsgSender.PushMsg(MsgDTO, "你的钻石余额不足，请添加能天使(2731544408)为好友后，使用【转账】功能转任意金额后将会获得金额*100的钻石，可以【我的状态】命令查看余额！");
                return false;
            }

            WorldLine.AIInstance<DeveloperOnlyAI>().ChargeTime(MsgDTO, new object[] {MsgDTO.FromGroup, (long)days});
            osPerson.Diamonds -= diamondNeed;
            osPerson.Update();

            return true;
        }
    }
}
