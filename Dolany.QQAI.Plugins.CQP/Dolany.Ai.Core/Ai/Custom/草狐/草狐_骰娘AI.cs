using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai.Custom.草狐
{
    public class 草狐_骰娘AI : AIBase
    {
        public override string AIName { get; set; } = "草狐_骰娘";
        public override string Description { get; set; } = "草狐定制骰娘";
        public override int PriorityLevel { get; set; } = 10;
        public override bool NeedManualOpeon { get; } = true;

        private readonly int DiceCountMaxLimit = Global.DefaultConfig.DiceCountMaxLimit;

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            MsgDTO.FullMsg = MsgDTO.FullMsg.Replace("。", ".");

            if (!ParseFormat(MsgDTO))
            {
                return false;
            }

            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = AIName,
                Command = "DiceOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });
            return true;
        }

        private bool ParseFormat(MsgInformationEx MsgDTO)
        {
            if (!MsgDTO.FullMsg.Contains(".r"))
            {
                return false;
            }

            if (MsgDTO.FullMsg.Trim() == ".r")
            {
                var randInt = Rander.RandInt(6) + 1;
                MsgSender.PushMsg(MsgDTO, $"你掷出了 {randInt}点！", true);
                return true;
            }

            var afterPart = MsgDTO.FullMsg.Replace(".r", string.Empty).Trim();
            if (int.TryParse(afterPart, out var extra))
            {
                var randInt = Rander.RandInt(6) + 1;
                MsgSender.PushMsg(MsgDTO, $"你掷出了 {randInt}+{extra}={randInt + extra}点！", true);
                return true;
            }

            if (!afterPart.StartsWith("*"))
            {
                return false;
            }

            var multiPart = afterPart.Replace("*", string.Empty).Trim();
            if (!int.TryParse(multiPart, out var multiNum) || multiNum <= 0 || multiNum > DiceCountMaxLimit)
            {
                return false;
            }

            var randInts = Enumerable.Range(0, multiNum).Select(p =>
            {
                Thread.Sleep(10);
                return Rander.RandInt(6) + 1;
            }).ToList();

            var intMsg = string.Join("+", randInts);
            var msg = $"你掷出了 {intMsg}={randInts.Sum()}点！";
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }
    }
}
