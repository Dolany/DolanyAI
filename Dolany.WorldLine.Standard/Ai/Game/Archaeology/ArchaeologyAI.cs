using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;
using Dolany.UtilityTool;
using Dolany.WorldLine.Standard.Ai.Game.Archaeology.ArchAdv;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchaeologyAI : AIBase
    {
        public override string AIName { get; set; } = "考古学";
        public override string Description { get; set; } = "Ai for Archaeology!";

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.考古学;

        public CrossWorldAiSvc CrossWorldAiSvc { get; set; }

        [EnterCommand(ID = "ArchaeologyAI_ConsumeRedStarStone",
            Command = "赤星归元",
            Description = "击碎一颗赤星石，刷新指定功能的CD",
            SyntaxHint = "[功能名]",
            SyntaxChecker = "Word")]
        public bool ConsumeRedStarStone(MsgInformationEx MsgDTO, object[] param)
        {
            var asset = ArchAsset.Get(MsgDTO.FromQQ);
            if (asset.RedStarStone == 0)
            {
                MsgSender.PushMsg(MsgDTO, "赤星石不足！", true);
                return false;
            }

            var command = param[1] as string;

            var enters = CrossWorldAiSvc[MsgDTO.FromGroup].AllAvailableGroupCommands.Where(p => p.Command == command).ToList();
            if (enters.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "未找到该功能！", true);
                return false;
            }

            var enter = enters.First();
            if (enters.Count > 1)
            {
                var options = enters.Select(p => $"{p.Command} {p.SyntaxHint}").ToArray();
                var response = WaiterSvc.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, "请选择需要刷新的功能：", options, MsgDTO.BindAi);
                if (response < 0)
                {
                    MsgSender.PushMsg(MsgDTO, "操作取消！");
                    return false;
                }

                enter = enters[response];
            }

            var dailyLimit = DailyLimitRecord.Get(MsgDTO.FromQQ, enter.ID);
            dailyLimit.Times = 0;
            dailyLimit.Update();

            asset.RedStarStone--;
            asset.Update();

            MsgSender.PushMsg(MsgDTO, "赤星归元(刷新成功)！");
            return true;
        }

        [EnterCommand(ID = "ArchaeologyAI_ConsumeRedStarStone_Re",
            Command = "赤星净化",
            Description = "击碎一颗赤星石，驱散自身所有的负面状态")]
        public bool ConsumeRedStarStone_Re(MsgInformationEx MsgDTO, object[] param)
        {
            var asset = ArchAsset.Get(MsgDTO.FromQQ);
            if (asset.RedStarStone == 0)
            {
                MsgSender.PushMsg(MsgDTO, "赤星石不足！", true);
                return false;
            }

            var buffs = OSPersonBuff.Get(MsgDTO.FromQQ);
            if (buffs.IsNullOrEmpty() || buffs.All(p => p.IsPositive))
            {
                MsgSender.PushMsg(MsgDTO, "你没有任何负面状态！");
                return false;
            }

            foreach (var buff in buffs.Where(p => !p.IsPositive))
            {
                buff.Remove();
            }

            asset.RedStarStone--;
            asset.Update();

            MsgSender.PushMsg(MsgDTO, "赤星净化(驱散成功)！");
            return true;
        }

        [EnterCommand(ID = "ArchaeologyAI_UpdateElement",
            Command = "元素晋升",
            Description = "升级元素力量(寒冰/火焰/雷电)")]
        public bool UpdateElement(MsgInformationEx MsgDTO, object[] param)
        {
            var archaeologist = Archaeologist.Get(MsgDTO.FromQQ);
            if (archaeologist.IsDead)
            {
                MsgSender.PushMsg(MsgDTO, $"你当前处在死亡惩罚时间中，无法进行该操作！复活时间：{archaeologist.RebornTime:yyyy-MM-dd HH:mm:ss}");
                return false;
            }

            var asset = ArchAsset.Get(MsgDTO.FromQQ);

            var flameEles = asset.FlameEssence;
            var iceEles = asset.IceEssence;
            var lightningEles = asset.LightningEssence;

            var flameNeed = archaeologist.Flame * 10;
            var iceNeed = archaeologist.Ice * 10;
            var lighningNeed = archaeologist.Lightning * 10;

            var options = new[]
            {
                $"{Emoji.火焰}：({flameEles}/{flameNeed})",
                $"{Emoji.雪花}：({iceEles}/{iceNeed})",
                $"{Emoji.闪电}：({lightningEles}/{lighningNeed})",
                "取消"
            };
            var option = WaiterSvc.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, "请选择你要晋升的元素之力！", options, MsgDTO.BindAi);
            if (option < 0 || option == 3)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            switch (option)
            {
                case 0 when flameEles < flameNeed:
                    MsgSender.PushMsg(MsgDTO, "火焰精魄不足，无法晋升！");
                    return false;
                case 0:
                    asset.FlameEssence -= flameNeed;
                    archaeologist.Flame++;
                    break;
                case 1 when iceEles < iceNeed:
                    MsgSender.PushMsg(MsgDTO, "寒冰精魄不足，无法晋升！");
                    return false;
                case 1:
                    asset.IceEssence -= iceNeed;
                    archaeologist.Ice++;
                    break;
                case 2 when lightningEles < lighningNeed:
                    MsgSender.PushMsg(MsgDTO, "雷电精魄不足，无法晋升！");
                    return false;
                case 2:
                    asset.LightningEssence -= lighningNeed;
                    archaeologist.Lightning++;
                    break;
            }

            asset.Update();
            archaeologist.Update();

            MsgSender.PushMsg(MsgDTO, "晋升成功！");
            return true;
        }

        [EnterCommand(ID = "ArchaeologyAI_Reborn",
            Command = "复活祈愿",
            Description = "击碎一颗赤星石，立刻从死亡状态中复活")]
        public bool Reborn(MsgInformationEx MsgDTO, object[] param)
        {
            var archaeologist = Archaeologist.Get(MsgDTO.FromQQ);
            if (!archaeologist.IsDead)
            {
                MsgSender.PushMsg(MsgDTO, "活着的人，是不需要复活的！");
                return false;
            }

            var asset = ArchAsset.Get(MsgDTO.FromQQ);
            if (asset.RedStarStone < 1)
            {
                MsgSender.PushMsg(MsgDTO, $"你没有足够的赤星石来复活！({asset.RedStarStone}/1)");
                return false;
            }

            var msg = $"此操作将会花费赤星石*1，你当前剩余赤星石({asset.RedStarStone})颗，是否确认？";
            if (!WaiterSvc.WaitForConfirm(MsgDTO, msg, 10))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            asset.RedStarStone--;
            archaeologist.RebornTime = DateTime.Now;

            asset.Update();
            archaeologist.Update();

            MsgSender.PushMsg(MsgDTO, "祈愿成功！");
            return true;
        }

        [EnterCommand(ID = "ArchaeologyAI_BlackJadeExchange",
            Command = "兑换墨玉",
            Description = "使用金币兑换墨玉（实时汇率！），一次限购100枚墨玉",
            DailyLimit = 3,
            TestingDailyLimit = 4)]
        public bool BlackJadeExchange(MsgInformationEx MsgDTO, object[] param)
        {
            var ratio = BlackJadeExchangeRec.RealTimeRatio(MsgDTO.FromGroup);
            var count = WaiterSvc.WaitForNum(MsgDTO.FromGroup, MsgDTO.FromQQ, $"当前墨玉汇率为：{ratio}金币 = 1墨玉，请输入兑换墨玉数量！（单次限购100枚墨玉）", bjCount => bjCount > 0 && bjCount <= 100,
                MsgDTO.BindAi);
            if (count <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var consumeTotal = count * ratio;
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < consumeTotal)
            {
                MsgSender.PushMsg(MsgDTO, $"你的金币余额不足！（{consumeTotal.CurencyFormat()}/{osPerson.Golds.CurencyFormat()}）");
                return false;
            }

            var asset = ArchAsset.Get(MsgDTO.FromQQ);
            asset.BlackJade += count;
            osPerson.Golds -= consumeTotal;

            asset.Update();
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, $"兑换成功！你当前剩余墨玉 {asset.BlackJade}枚，金币 {osPerson.Golds.CurencyFormat()}！");
            return true;
        }

        [EnterCommand(ID = "ArchaeologyAI_MyAsset",
            Command = "我的考古资产",
            Description = "查看自己的考古资产")]
        public bool MyAsset(MsgInformationEx MsgDTO, object[] param)
        {
            var asset = ArchAsset.Get(MsgDTO.FromQQ);
            var msgList = new List<string>()
            {
                "你当前持有的考古资产有：",
                $"翠绿琥珀：{asset.GreenAmbur}",
                $"碧蓝琥珀：{asset.BlueAmbur}",
                $"墨玉：{asset.BlackJade}",
                $"赤星石：{asset.RedStarStone}",
                $"寒冰元素精魄：{asset.IceEssence}",
                $"火焰元素精魄：{asset.FlameEssence}",
                $"雷电元素精魄：{asset.LightningEssence}"
            };

            var msg = string.Join("\r\n", msgList);
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "ArchaeologyAI_MyStatus", Command = "我的考古状态", Description = "查看自己的考古状态")]
        public bool MyStatus(MsgInformationEx MsgDTO, object[] param)
        {
            var archaeologist = Archaeologist.Get(MsgDTO.FromQQ);
            var collection = ArchCollection.Get(MsgDTO.FromQQ);
            var msgList = new List<string>()
            {
                $"SAN:{archaeologist.CurSAN}/{archaeologist.SAN}",
                $"{Emoji.雪花}:{archaeologist.Ice}",
                $"{Emoji.火焰}:{archaeologist.Flame}",
                $"{Emoji.闪电}:{archaeologist.Lightning}",
                $"收藏品碎片:{collection.ItemColles.Sum(p => p.Segments.Sum(s => s.Value))}",
                $"收藏品:{collection.Collectables.Sum(p => p.Value)}",
                $"特殊收藏品:{collection.SpecialColles.Count}",
                $"地图碎片:{collection.MapSegments}"
            };
            if (archaeologist.IsDead)
            {
                msgList.Add($"复活时间:{archaeologist.RebornTime:yyyy-MM-dd HH:mm:ss}");
            }

            var msg = string.Join("\r\n", msgList);
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "ArchaeologyAI_StartAdv", Command = "考古冒险", Description = "开始一场考古冒险！（需要消耗琥珀）")]
        public bool StartAdv(MsgInformationEx MsgDTO, object[] param)
        {
            var archaeologist = Archaeologist.Get(MsgDTO.FromQQ);
            if (archaeologist.IsDead)
            {
                MsgSender.PushMsg(MsgDTO, $"你当前处在死亡惩罚时间中，无法进行该操作！复活时间：{archaeologist.RebornTime:yyyy-MM-dd HH:mm:ss}");
                return false;
            }

            var asset = ArchAsset.Get(MsgDTO.FromQQ);
            if (asset.GreenAmbur == 0 && asset.BlueAmbur == 0)
            {
                MsgSender.PushMsg(MsgDTO, "很抱歉，你没有任何琥珀，无法开启副本！");
                return false;
            }

            if (asset.GreenAmbur > 0)
            {
                asset.GreenAmbur -= 1;
            }
            else
            {
                if (!WaiterSvc.WaitForConfirm(MsgDTO, $"此操作将消耗 碧蓝琥珀*1 （你当前剩余 {asset.BlueAmbur}），是否继续？"))
                {
                    MsgSender.PushMsg(MsgDTO, "操作取消！");
                    return false;
                }

                asset.BlueAmbur -= 1;
            }

            asset.Update();

            var engine = new ArchAdvEngine(MsgDTO);
            engine.StartAdv();

            return true;
        }
    }
}
