using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Sys.BonusCenter
{
    [AI(
        Name = "兑奖中心",
        Description = "AI for auto bonus.",
        Enable = true,
        PriorityLevel = 10)]
    public class BonusCenterAI : AIBase
    {
        private Dictionary<string, BonusBase> BonusDic = new Dictionary<string, BonusBase>();

        public override void Initialization()
        {
            var assembly = Assembly.GetAssembly(typeof(BonusBase));
            var list = assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(BonusBase)))
                .Where(type => type.FullName != null)
                .Select(type => assembly.CreateInstance(type.FullName) as BonusBase);

            BonusDic = list.ToDictionary(p => p.Code, p => p);
        }

        [EnterCommand(Command = "兑奖",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "根据兑换码兑换奖励",
            Syntax = "[兑换码]",
            SyntaxChecker = "Word",
            Tag = "兑奖",
            IsPrivateAvailable = true)]
        public bool AutoBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var code = param[0] as string;
            if (!BonusDic.ContainsKey(code))
            {
                MsgSender.PushMsg(MsgDTO, "兑换码错误");
                return false;
            }

            var bonus = BonusDic[code];
            if (bonus.IsExpiried)
            {
                MsgSender.PushMsg(MsgDTO, "兑换码已过期");
                return false;
            }

            if (!AutoBonusRecord.Check(MsgDTO.FromQQ, code))
            {
                MsgSender.PushMsg(MsgDTO, "你已经使用过该兑换码");
                return false;
            }

            if (!bonus.SendBonus(MsgDTO))
            {
                return false;
            }

            AutoBonusRecord.Record(MsgDTO.FromQQ, code);
            MsgSender.PushMsg(MsgDTO, "兑换成功");

            return true;
        }
    }
}
