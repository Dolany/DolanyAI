using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;
using Dolany.UtilityTool;
using Dolany.WorldLine.Standard.Ai.Game.Pet;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.SegmentAttach
{
    public class SegmentAttachAI : AIBase
    {
        public override string AIName { get; set; } = "碎片拼接";
        public override string Description { get; set; } = "AI for segments attaching game.";
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.宝藏功能;

        public SegmentSvc SegmentSvc { get; set; }
        public HonorSvc HonorSvc { get; set; }
        public BindAiSvc BindAiSvc { get; set; }

        [EnterCommand(ID = "SegmentAttachAI_TakeSegment",
            Command = "领取宝藏碎片",
            Description = "领取一张宝藏碎片（和另一块碎片拼接后，将获得宝藏！）",
            IsPrivateAvailable = true,
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool TakeSegment(MsgInformationEx MsgDTO, object[] param)
        {
            var segment = SegmentSvc.RandSegment();
            var record = SegmentRecord.Get(MsgDTO.FromQQ);
            record.Segment = segment.Name;
            record.IsRare = Rander.RandInt(100) > 90;
            record.Update();

            var msg = $"你领取到了新的宝藏碎片！\r\n{segment}";
            var treasure = SegmentSvc.FindTreasureBySegment(record.Segment);
            msg += $"\r\n可开启宝藏：【{treasure.Name}】";
            if (record.IsRare)
            {
                msg += $"\r\n{Emoji.礼物}恭喜你领取到了稀有碎片，拼接后将得到双倍奖励！";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_ViewSegment",
            Command = "查看宝藏碎片",
            Description = "查看指定的宝藏碎片的信息",
            SyntaxHint = "[宝藏碎片名称]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ViewSegment(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var segment = SegmentSvc.FindSegmentByName(name);
            if (segment == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到指定的宝藏碎片");
                return false;
            }

            var treasure = SegmentSvc.FindTreasureBySegment(name);
            var msg = $"{segment}\r\n可开启宝藏：{treasure.Name}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_MySegment",
            Command = "我的宝藏碎片",
            Description = "查看自己持有的宝藏碎片的信息",
            IsPrivateAvailable = true)]
        public bool MySegment(MsgInformationEx MsgDTO, object[] param)
        {
            var record = SegmentRecord.Get(MsgDTO.FromQQ);
            if (string.IsNullOrEmpty(record.Segment))
            {
                MsgSender.PushMsg(MsgDTO, "你尚未持有任何宝藏碎片！", true);
                return false;
            }
            var segment = SegmentSvc.FindSegmentByName(record.Segment);
            if (segment == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到指定的宝藏碎片");
                return false;
            }

            var treasure = SegmentSvc.FindTreasureBySegment(record.Segment);
            var msg = $"{segment}\r\n可开启宝藏：{treasure.Name}";
            if (record.IsRare)
            {
                msg += "\r\n【稀有】：拼接后将得到双倍奖励！";
            }

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_ViewTreasure",
            Command = "查看宝藏",
            Description = "查看指定的宝藏的信息",
            SyntaxHint = "[宝藏名称]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ViewTreasure(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;

            var treasure = SegmentSvc.FindTreasureByName(name);
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
            Description = "查看自己开启过的宝藏记录",
            IsPrivateAvailable = true)]
        public bool MyTreasureRecord(MsgInformationEx MsgDTO, object[] param)
        {
            var record = SegmentRecord.Get(MsgDTO.FromQQ);
            if (record.TreasureRecord.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你还没有开启过任何宝藏！");
                return false;
            }

            var recMsgList = record.TreasureRecord.Select(p => $"{p.Key}：{p.Value}次").ToList();
            recMsgList.Add($"总计：{record.TreasureRecord.Sum(p => p.Value)}次");
            var finalMsg = $"终极宝藏：{record.FinalTreasureCount}次";
            if (record.CanOpenFinalTreasure)
            {
                finalMsg += $"(还可开启{record.TreasureRecord.Values.Min() - record.FinalTreasureCount}次)";
            }
            recMsgList.Add(finalMsg);
            var msg = string.Join("\r\n", recMsgList);
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_OpenFinalTreasure",
            Command = "开启终极宝藏",
            Description = "开启过所有宝藏之后，可以开启传说中的终极宝藏！",
            IsPrivateAvailable = true)]
        public bool OpenFinalTreasure(MsgInformationEx MsgDTO, object[] param)
        {
            var record = SegmentRecord.Get(MsgDTO.FromQQ);
            if (!record.CanOpenFinalTreasure)
            {
                MsgSender.PushMsg(MsgDTO, "很遗憾，你还不能开启终极宝藏，继续努力吧！（开启过所有其他宝藏才能开启终极宝藏！）", true);
                return false;
            }

            var options = new[] {"获取500金币", "随机获取商店售卖的一件商品*5", "宠物获取50点经验值", "捞瓶子机会*5(仅当日有效)"};
            var selectedIdx = WaiterSvc.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, "请选择你要开启的宝藏：", options, MsgDTO.BindAi);
            if (selectedIdx < 0)
            {
                MsgSender.PushMsg(MsgDTO, "你已经放弃了思考！");
                return false;
            }

            switch (selectedIdx)
            {
                case 0:
                {
                    var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
                    osPerson.Golds += 500;
                    osPerson.Update();

                    MsgSender.PushMsg(MsgDTO, $"恭喜你获得了 {500.CurencyFormat()}！");
                    break;
                }
                case 1:
                {
                    var items = TransHelper.GetDailySellItems();
                    var randItem = items.RandElement();

                    MsgSender.PushMsg(MsgDTO, $"恭喜你获得了 {randItem.Name}*5！");

                    var collo = ItemCollectionRecord.Get(MsgDTO.FromQQ);
                    var msg = collo.ItemIncome(randItem.Name, 5);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        MsgSender.PushMsg(MsgDTO, msg);
                    }
                    break;
                }
                case 2:
                {
                    var pet = PetRecord.Get(MsgDTO.FromQQ);
                    var msg = pet.ExtGain(MsgDTO, 50);
                    MsgSender.PushMsg(MsgDTO, msg);
                    break;
                }
                case 3:
                {
                    var dailyLimit = DailyLimitRecord.Get(MsgDTO.FromQQ, "DriftBottleAI_FishingBottle");
                    dailyLimit.Decache(5);
                    dailyLimit.Update();

                    MsgSender.PushMsg(MsgDTO, "恭喜你获取 捞瓶子机会*5(仅当日有效) ！");
                    break;
                }
            }

            record.FinalTreasureCount++;
            record.Update();

            return true;
        }

        [EnterCommand(ID = "SegmentAttachAI_AttachSegment",
            Command = "拼接宝藏碎片",
            Description = "和其他成员一起拼接宝藏碎片",
            SyntaxHint = "[@QQ号]",
            SyntaxChecker = "At")]
        public bool AttachSegment(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            if (aimQQ == MsgDTO.FromQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你无法和自己进行拼接！", true);
                return false;
            }

            if (BindAiSvc.AllAiNums.Contains(aimQQ))
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

            var treasure = SegmentSvc.FindTreasureBySegment(selfRecord.Segment);
            if (!treasure.IsMatch(selfRecord.Segment, aimRecord.Segment))
            {
                MsgSender.PushMsg(MsgDTO, $"拼接失败，碎片不匹配！({selfRecord.Segment}×{aimRecord.Segment})");
                return false;
            }

            selfRecord.AddTreasureRecord(treasure.Name);
            aimRecord.AddTreasureRecord(treasure.Name);

            var selfBonusItems = HonorSvc.RandItems(3);
            var aimBonusItems = HonorSvc.RandItems(3);

            var selfIcRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var aimIcRecord = ItemCollectionRecord.Get(aimQQ);

            foreach (var item in selfBonusItems)
            {
                selfIcRecord.ItemIncome(item.Name, selfRecord.IsRare ? 2 : 1);
            }

            foreach (var item in aimBonusItems)
            {
                aimIcRecord.ItemIncome(item.Name, aimRecord.IsRare ? 2 : 1);
            }

            MsgSender.PushMsg(MsgDTO, treasure.ToString());

            var msg = "拼接成功！\r\n" +
                      $"{CodeApi.Code_At(MsgDTO.FromQQ)} 获得了{string.Join(",", selfBonusItems.Select(p => $"{p.Name}*{(selfRecord.IsRare ? 2 : 1)}"))} ！\r\n" +
                      $"{CodeApi.Code_At(aimQQ)} 获得了{string.Join(",", aimBonusItems.Select(p => $"{p.Name}*{(aimRecord.IsRare ? 2 : 1)}"))} ！";
            MsgSender.PushMsg(MsgDTO, msg);

            selfRecord.ClearSegment();
            aimRecord.ClearSegment();
            selfRecord.Update();
            aimRecord.Update();

            return true;
        }
    }
}
