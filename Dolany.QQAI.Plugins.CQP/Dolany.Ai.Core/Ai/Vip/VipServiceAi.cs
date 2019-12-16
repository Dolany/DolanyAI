using System.Linq;
using Dolany.Ai.Common;
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
            Command = "vip商店 钻石商店",
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
            var goodsMsg = string.Join("\r", goods.Select(g => $"{g.Name}({g.DiamondsNeed}{Emoji.钻石}):{g.Description}"));
            var msg = $"今天提供的vip服务有：\r{goodsMsg}\r你当前余额为：{osPerson.Diamonds}{Emoji.钻石}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "VipServiceAi_ViewArmer",
            Command = "查看装备",
            Description = "查看指定名称的装备",
            Syntax = "[装备名称]",
            Tag = "vip服务",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true)]
        public bool ViewArmer(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var armer = DailyVipShopMgr.Instance[name];
            if (armer == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到相关装备！");
                return false;
            }

            var msg = $"{armer.Name}\r    {armer.Description}\r售价：{armer.DiamondsNeed}{Emoji.钻石}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "VipServiceAi_MyArmer",
            Command = "我的装备",
            Description = "查看自己拥有的装备",
            Syntax = "[装备名称]",
            Tag = "vip服务",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true)]
        public bool MyArmer(MsgInformationEx MsgDTO, object[] param)
        {
            var record = VipArmerRecord.Get(MsgDTO.FromQQ);
            if (record.Armers.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未持有任何装备！");
                return false;
            }

            var armerMsgs = record.Armers.Select(r => $"{r.Name}：{r.Description}{(r.ExpiryTime.HasValue ? $"({r.ExpiryTime})" : string.Empty)}");
            var msg = $"你当前持有的装备有：\r{string.Join("\r", armerMsgs)}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
