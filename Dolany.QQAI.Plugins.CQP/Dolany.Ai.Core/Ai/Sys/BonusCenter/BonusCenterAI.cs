using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Sys.BonusCenter
{
    [AI(Name = "兑奖中心",
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

            BonusDic = list.ToDictionary(p => p?.Code, p => p);
        }

        [EnterCommand(ID = "BonusCenterAI_AutoBonus",
            Command = "兑奖",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "根据兑换码兑换奖励",
            Syntax = "[兑换码]",
            SyntaxChecker = "Word",
            Tag = "系统命令",
            IsPrivateAvailable = true)]
        public bool AutoBonus(MsgInformationEx MsgDTO, object[] param)
        {
            var code = param[0] as string;
            var codeRef = BonusCodeRef.Get(code);
            if (codeRef == null)
            {
                MsgSender.PushMsg(MsgDTO, "无效的兑换码！", true);
                return false;
            }

            if (!BonusDic.ContainsKey(codeRef.Ref))
            {
                MsgSender.PushMsg(MsgDTO, "该兑换码对应的奖励已下架！", true);
                return false;
            }

            var bonus = BonusDic[codeRef.Ref];
            if (!bonus.SendBonus(MsgDTO))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            codeRef.Remove();
            MsgSender.PushMsg(MsgDTO, "兑换成功！", true);
            return true;
        }
    }
}
