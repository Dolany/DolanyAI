using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.SegmentAttach
{
    public class SegmentAttachAI : AIBase
    {
        public override string AIName { get; set; } = "碎片拼接";
        public override string Description { get; set; } = "AI for segments attaching game.";
        public override int PriorityLevel { get; set; } = 10;

        [EnterCommand(ID = "SegmentAttachAI_TakeSegment",
            Command = "领取宝藏碎片",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "领取一张宝藏碎片（和另一块碎片拼接后，将获得宝藏！）",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "宝藏功能",
            IsPrivateAvailable = true,
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool TakeSegment(MsgInformationEx MsgDTO, object[] param)
        {
            var segment = SegmentMgr.Instance.RandSegment();
            var record = SegmentRecord.Get(MsgDTO.FromQQ);
            record.Segment = segment.Name;
            record.Update();

            var msg = $"你领取到了新的宝藏碎片！\r{segment}";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_ViewSegment",
            Command = "查看宝藏碎片",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看指定的宝藏碎片的信息",
            Syntax = "[宝藏碎片名称]",
            SyntaxChecker = "Word",
            Tag = "宝藏功能",
            IsPrivateAvailable = true)]
        public bool ViewSegment(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var segment = SegmentMgr.Instance.FindSegmentByName(name);
            if (segment == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到指定的宝藏碎片");
                return false;
            }

            var treasure = SegmentMgr.Instance.FindTreasureBySegment(name);
            var msg = $"{segment}\r可开启宝藏：{treasure.Name}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_MySegment",
            Command = "我的宝藏碎片",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己持有的宝藏碎片的信息",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "宝藏功能",
            IsPrivateAvailable = true)]
        public bool MySegment(MsgInformationEx MsgDTO, object[] param)
        {
            var record = SegmentRecord.Get(MsgDTO.FromQQ);
            if (string.IsNullOrEmpty(record.Segment))
            {
                MsgSender.PushMsg(MsgDTO, "你尚未持有任何宝藏碎片！", true);
                return false;
            }
            var segment = SegmentMgr.Instance.FindSegmentByName(record.Segment);
            if (segment == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到指定的宝藏碎片");
                return false;
            }

            var treasure = SegmentMgr.Instance.FindTreasureBySegment(record.Segment);
            var msg = $"{segment}\r可开启宝藏：{treasure.Name}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_ViewTreasure",
            Command = "查看宝藏",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看指定的宝藏的信息",
            Syntax = "[宝藏名称]",
            SyntaxChecker = "Word",
            Tag = "宝藏功能",
            IsPrivateAvailable = true)]
        public bool ViewTreasure(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;

            var treasure = SegmentMgr.Instance.FindTreasureByName(name);
            if (treasure == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到指定的宝藏");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, treasure.ToString());
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_MyTreasureRecord",
            Command = "我的宝藏记录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己开启过的宝藏记录",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "宝藏功能",
            IsPrivateAvailable = true)]
        public bool MyTreasureRecord(MsgInformationEx MsgDTO, object[] param)
        {
            var record = SegmentRecord.Get(MsgDTO.FromQQ);
            if (record.TreasureRecord.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你还没有开启过任何宝藏！");
                return false;
            }

            var msg = string.Join("\r", record.TreasureRecord.Select(p => $"{p.Key}：{p.Value}次"));
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_AttachSegment",
            Command = "拼接宝藏碎片",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "和其他成员一起拼接宝藏碎片",
            Syntax = "[@QQ号]",
            SyntaxChecker = "At",
            Tag = "宝藏功能",
            IsPrivateAvailable = false)]
        public bool AttachSegment(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            if (aimQQ == MsgDTO.FromQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你无法和自己进行拼接！", true);
                return false;
            }

            if (BindAiMgr.Instance.AllAiNums.Contains(aimQQ))
            {
                MsgSender.PushMsg(MsgDTO, "Stupid Human!", true);
                return false;
            }

            var selfRecord = SegmentRecord.Get(MsgDTO.FromQQ);
            if (string.IsNullOrEmpty(selfRecord.Segment))
            {
                MsgSender.PushMsg(MsgDTO, "你当前没有任何碎片！", true);
                return false;
            }

            var aimRecord = SegmentRecord.Get(aimQQ);
            if (string.IsNullOrEmpty(aimRecord.Segment))
            {
                MsgSender.PushMsg(MsgDTO, "对方当前没有任何碎片！", true);
                return false;
            }

            var treasure = SegmentMgr.Instance.FindTreasureBySegment(selfRecord.Segment);
            if (!treasure.IsMatch(selfRecord.Segment, aimRecord.Segment))
            {
                MsgSender.PushMsg(MsgDTO, $"拼接失败，碎片不匹配！({selfRecord.Segment}×{aimRecord.Segment})");
                return false;
            }

            selfRecord.Segment = string.Empty;
            aimRecord.Segment = string.Empty;

            selfRecord.AddTreasureRecord(treasure.Name);
            aimRecord.AddTreasureRecord(treasure.Name);

            var selfBonusItems = HonorHelper.Instance.RandItems(3);
            var aimBonusItems = HonorHelper.Instance.RandItems(3);

            var selfIcRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var aimIcRecord = ItemCollectionRecord.Get(aimQQ);

            foreach (var item in selfBonusItems)
            {
                selfIcRecord.ItemIncome(item.Name);
            }

            foreach (var item in aimBonusItems)
            {
                aimIcRecord.ItemIncome(item.Name);
            }

            selfRecord.Update();
            aimRecord.Update();

            MsgSender.PushMsg(MsgDTO, treasure.ToString());

            var msg = $"拼接成功！\r" +
                      $"{CodeApi.Code_At(MsgDTO.FromQQ)} 获得了{string.Join(",", selfBonusItems.Select(p => p.Name))} ！\r" +
                      $"{CodeApi.Code_At(aimQQ)} 获得了{string.Join(",", aimBonusItems.Select(p => p.Name))} ！";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
