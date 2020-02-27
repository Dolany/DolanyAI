using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Vip
{
    public class VipServiceAi : AIBase
    {
        public override string AIName { get; set; } = "Vip服务";
        public override string Description { get; set; } = "Ai for vip services.";
        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        public DailyVipShopMgr DailyVipShopMgr { get; set; }

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
            var goods = goodsName.Select(g => DailyVipShopMgr[g]).ToList();
            var goodsMsg = string.Join("\r", goods.Select(g => $"{g.Name}({g.DiamondsNeed.CurencyFormat("Diamond")}):{g.Description}"));
            var msg = $"今天提供的vip服务有：\r{goodsMsg}\r你当前余额为：{osPerson.Diamonds.CurencyFormat("Diamond")}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "VipServiceAi_RefreshVipShop",
            Command = "刷新vip商店 刷新钻石商店",
            Description = "刷新vip商店（花费10钻石）",
            Syntax = "",
            Tag = "vip服务",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true)]
        public bool RefreshVipShop(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Diamonds < 10)
            {
                MsgSender.PushMsg(MsgDTO, $"很抱歉，你当前的钻石余额不足，无法刷新vip商店！({osPerson.Diamonds}/10)");
                return false;
            }

            if (!Waiter.WaitForConfirm(MsgDTO, $"此操作将花费{10.CurencyFormat("Diamond")}，是否继续？"))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            osPerson.Diamonds -= 10;
            osPerson.Update();

            DailyVipGoodsRecord.Refresh(MsgDTO.FromQQ);

            MsgSender.PushMsg(MsgDTO, "刷新成功！");
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
            var armer = DailyVipShopMgr[name];
            if (armer == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到相关装备！");
                return false;
            }

            var msg = $"{armer.Name}\r    {armer.Description}\r售价：{armer.DiamondsNeed.CurencyFormat("Diamond")}";
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

        public override bool OnMoneyReceived(ChargeModel model)
        {
            var diamonds = (int) (Math.Round(model.Amount, 2) * 100);
            var chargeRec = new VipChargeRecord()
            {
                QQNum = model.QQNum,
                ChargeAmount = model.Amount,
                ChargeTime = DateTime.Now,
                DiamondAmount = diamonds,
                Message = model.Message,
                OrderID = model.OrderID
            };
            chargeRec.Insert();

            var osPerson = OSPerson.GetPerson(model.QQNum);
            osPerson.Diamonds += diamonds;
            osPerson.Update();

            MsgSender.PushMsg(0, model.QQNum, $"恭喜充值成功！当前余额：{osPerson.Diamonds}{Emoji.钻石}", model.BindAi);
            MsgSender.PushMsg(0, Global.DeveloperNumber, $"{model.QQNum}充值{model.Amount}元！", Global.DefaultConfig.MainAi);

            return true;
        }
    }
}
