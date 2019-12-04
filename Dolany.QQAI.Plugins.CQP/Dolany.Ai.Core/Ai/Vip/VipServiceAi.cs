using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class VipServiceAi : AIBase
    {
        public override string AIName { get; set; } = "Vip服务";
        public override string Description { get; set; } = "Ai for vip services.";
        public override int PriorityLevel { get; set; } = 10;

        [EnterCommand(ID = "VipServiceAi_VipShop",
            Command = "vip商店",
            Description = "打开vip商店(每日刷新7个服务项目)",
            Syntax = "",
            Tag = "vip服务",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true)]
        public bool VipShop(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Diamonds <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "很抱歉，你当前的钻石余额不足，无法打开vip商店！");
                return false;
            }

            var goodsName = DailyVipGoodsRecord.GetToday(MsgDTO.FromQQ).GoodsName;
            var goods = goodsName.Select(g => DailyVipShopMgr.Instance[g]).ToList();
            var goodsMsg = string.Join("\r", goods.Select(g => $"{g.Name}:{g.DiamondsNeed}({g.DiamondsNeed}{Emoji.钻石})"));
            var msg = $"今天提供的vip服务有：\r{goodsMsg}\r你当前余额为：{osPerson.Diamonds}{Emoji.钻石}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
