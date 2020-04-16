using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;

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

        [EnterCommand(ID = "ArchaeologyAI_UpdateElement",
            Command = "元素晋升",
            Description = "升级元素力量(寒冰/火焰/雷电)")]
        public bool UpdateElement(MsgInformationEx MsgDTO, object[] param)
        {
            var asset = ArchAsset.Get(MsgDTO.FromQQ);
            var archaeologist = Archaeologist.Get(MsgDTO.FromQQ);

            var options = new[] {$"{Emoji.火焰}：({asset.ElementEssence["Flame"]}/{archaeologist.Flame * 10})"};
            var option = WaiterSvc.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, "请选择你要晋升的元素之力！", options, MsgDTO.BindAi);

            return true;
        }
    }
}
