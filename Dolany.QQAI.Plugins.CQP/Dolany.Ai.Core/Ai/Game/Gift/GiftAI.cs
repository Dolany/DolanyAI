using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Game.Gift
{
    [AI(Name = "礼物",
        Description = "AI for Gifts.",
        Enable = false,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class GiftAI : AIBase
    {
        [EnterCommand(ID = "GiftAI_MakeGift",
            Command = "制作礼物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "制作指定的礼物",
            Syntax = "[礼物名]",
            Tag = "礼物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool MakeGift(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var gift = GiftMgr.Instance[name];
            if (gift == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到该礼物！");
                return false;
            }

            var itemRecord = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var mdic = itemRecord.ItemCount.ToDictionary(p => p.Name, p => p.Count);
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (!gift.Check(mdic, osPerson.Golds, out var msg))
            {
                MsgSender.PushMsg(MsgDTO, $"制作{name}需要：\r{msg}材料不足，无法制作！");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, $"制作{name}需要：\r{msg}是否制作？", 7))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            itemRecord.ItemConsume(gift.MaterialDic);
            itemRecord.Update();
            osPerson.Golds -= gift.GoldNeed;
            osPerson.GiftIncome(name);
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "制作成功！", true);

            return true;
        }

        [EnterCommand(ID = "GiftAI_MyGifts",
            Command = "我的礼物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己拥有的礼物",
            Syntax = "",
            Tag = "礼物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            IsTesting = true)]
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
    }
}
