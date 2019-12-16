using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Gift
{
    public class GiftAI : AIBase
    {
        public override string AIName { get; set; } = "礼物";

        public override string Description { get; set; } = "AI for Gifts.";

        public override int PriorityLevel { get; set; } = 10;

        public override bool NeedManualOpeon { get; } = true;

        [EnterCommand(ID = "GiftAI_MakeGift",
            Command = "兑换礼物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "兑换指定的礼物",
            Syntax = "[礼物名]",
            Tag = "礼物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 3,
            TestingDailyLimit = 3)]
        public bool MakeGift(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var gift = GiftMgr.Instance[name];
            if (gift == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到该礼物！");
                return false;
            }

            var sellingGifts = GiftMgr.Instance.SellingGifts;
            if (sellingGifts.All(p => p.Name != name))
            {
                MsgSender.PushMsg(MsgDTO, "该礼物未在礼物商店中出售，请使用 礼物商店 命令查看今日可兑换的礼物！", true);
                return false;
            }

            var itemRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var mdic = itemRecord.HonorCollections.SelectMany(p => p.Value.Items).ToDictionary(p => p.Key, p => p.Value);
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (!gift.Check(mdic, osPerson.Golds, out var msg))
            {
                MsgSender.PushMsg(MsgDTO, $"兑换{name}需要：\r{msg}材料不足，无法兑换！");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, $"兑换{name}需要：\r{msg}是否兑换？", 7))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            itemRecord.ItemConsume(gift.MaterialDic);
            itemRecord.Update();
            osPerson.Golds -= gift.GoldNeed;
            osPerson.GiftIncome(name);
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "兑换成功！可以使用 赠送礼物 命令将礼物送给其他人！", true);

            return true;
        }

        [EnterCommand(ID = "GiftAI_MyGifts",
            Command = "我的礼物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己拥有的礼物",
            Syntax = "",
            Tag = "礼物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool MyGifts(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.GiftDic.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你当前没有任何礼物！", true);
                return false;
            }

            var msg = string.Join(",", osPerson.GiftDic.Select(p => $"{p.Key}*{p.Value}"));
            MsgSender.PushMsg(MsgDTO, $"你当前持有的礼物：\r{msg}");

            return true;
        }

        [EnterCommand(ID = "GiftAI_GiftShop",
            Command = "礼物商店",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "浏览礼物商店，查看今日可兑换的礼物",
            Syntax = "",
            Tag = "礼物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool GiftShop(MsgInformationEx MsgDTO, object[] param)
        {
            var sellingGifts = GiftMgr.Instance.SellingGifts;
            var msg = sellingGifts.Aggregate("当前可兑换的礼物有(礼物名/羁绊值/魅力值)：\r", (current, gift) => current + $"{gift.Name}/{gift.Intimate}/{gift.Glamour}\r");

            msg += "可以使用 查看礼物 [礼物名] 命令来查看详细信息；或者使用 兑换礼物 [礼物名] 命令来兑换指定礼物";
            MsgSender.PushMsg(MsgDTO, msg);

            return true;
        }

        [EnterCommand(ID = "GiftAI_ViewGift",
            Command = "查看礼物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看礼物的详细信息",
            Syntax = "[礼物名]",
            Tag = "礼物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ViewGift(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var gift = GiftMgr.Instance[name];
            if (gift == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到该礼物！");
                return false;
            }

            var msg = $"礼物名：{gift.Name}\r";
            msg += $"描述：{gift.Description}\r";
            msg += $"羁绊值：{gift.Intimate}\r";
            msg += $"魅力值：{gift.Glamour}\r";
            msg += $"兑换需要材料：{string.Join(",", gift.MaterialDic.Select(p => $"{p.Key}*{p.Value}"))}\r";
            msg += $"兑换需要金币：{gift.GoldNeed}";

            MsgSender.PushMsg(MsgDTO, msg);

            return true;
        }

        [EnterCommand(ID = "GiftAI_ViewRelationship",
            Command = "查看羁绊",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看和指定成员的羁绊值",
            Syntax = "[@QQ号]",
            Tag = "礼物功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false)]
        public bool ViewRelationship(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            if (aimQQ == MsgDTO.FromQQ)
            {
                MsgSender.PushMsg(MsgDTO, "和自己的羁绊，永远是0！", true);
                return false;
            }

            var relationship = IntimateRelationshipRecord.GetSumIntimate(MsgDTO.FromGroup, MsgDTO.FromQQ, aimQQ);
            MsgSender.PushMsg(MsgDTO, $"你们之间的羁绊值是：{relationship} ！");

            return true;
        }

        [EnterCommand(ID = "GiftAI_PresentGift",
            Command = "赠送礼物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "赠送某个成员一个礼物",
            Syntax = "[@QQ号] [礼物名]",
            Tag = "礼物功能",
            SyntaxChecker = "At Word",
            IsPrivateAvailable = false,
            DailyLimit = 3,
            TestingDailyLimit = 3)]
        public bool PresentGift(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            var name = param[1] as string;
            if (aimQQ == MsgDTO.FromQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你居然给自己送礼物！", true);
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.GiftDic == null || !osPerson.GiftDic.ContainsKey(name))
            {
                MsgSender.PushMsg(MsgDTO, "你没有这个礼物！", true);
                return false;
            }

            var gift = GiftMgr.Instance[name];
            osPerson.GiftDic[name]--;
            osPerson.Update();

            var glamourRecord = GlamourRecord.Get(MsgDTO.FromGroup, aimQQ);
            glamourRecord.Glamour += gift.Glamour;
            glamourRecord.Update();

            var relationship = new IntimateRelationshipRecord()
            {
                GroupNum = MsgDTO.FromGroup,
                QQPair = new []{MsgDTO.FromQQ, aimQQ},
                Value = gift.Intimate,
                Name = gift.Name
            };
            relationship.Insert();

            var msg = $"赠送成功！对方增加了 {gift.Glamour} 点魅力值，你们之间的羁绊值增加了 {gift.Intimate} 点！";
            MsgSender.PushMsg(MsgDTO, msg, true);

            return true;
        }
    }
}
