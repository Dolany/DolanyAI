using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.Ai.Game.SignIn;

namespace Dolany.WorldLine.Standard.Ai.Vip.VipArmers
{
    public class 补签卡 : IVipArmer
    {
        public string Name { get; set; } = "补签卡";
        public string Description { get; set; } = "补签当前群组最近一次漏签的签到（不算今天），每天可购买三次";
        public int DiamondsNeed { get; set; } = 10;
        public VipArmerLimitInterval LimitInterval { get; set; } = VipArmerLimitInterval.Daily;
        public int LimitCount { get; set; } = 3;
        public bool Purchase(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.FromGroup == 0)
            {
                MsgSender.PushMsg(MsgDTO, "只能在群组中购买该物品！", true);
                return false;
            }

            var rec = SignInSuccessiveRecord.MakeUp(MsgDTO.FromGroup, MsgDTO.FromQQ);
            if (rec == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到签到记录，无法补签！", true);
                return false;
            }

            var msg = $"补签成功！你当前连续签到 {rec.SuccessiveDays}天！";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        public int MaxContains { get; set; }
    }
}
