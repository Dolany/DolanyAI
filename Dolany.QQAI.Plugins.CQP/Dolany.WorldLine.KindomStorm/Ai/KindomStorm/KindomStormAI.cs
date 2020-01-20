using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class KindomStormAI : AIBase
    {
        public override string AIName { get; set; } = "王国风云";
        public override string Description { get; set; }

        [EnterCommand(ID = "KindomStormAI_MyCastle",
            Command = "我的城堡",
            Description = "查看自己的城堡的情况",
            Syntax = "",
            Tag = "王国风云",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = false)]
        public bool MyCastle(MsgInformationEx MsgDTO, object[] param)
        {
            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            var msg = $"{castle.CastleName}";
            msg += $"\r金钱：{castle.Golds}";
            msg += $"\r粮草：{castle.Commissariat}";
            msg += $"\r建筑：{string.Join(",", castle.Buildings.Select(p => $"{p.Key}(lv.{p.Value})"))}";

            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }
    }
}
