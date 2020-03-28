using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

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
            Tag = CmdTagEnum.王国风云,
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = false)]
        public bool MyCastle(MsgInformationEx MsgDTO, object[] param)
        {
            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            var msg = $"{castle.CastleName}";
            msg += $"\r\n城堡等级：{Utility.LevelEmoji(castle.Level)}";
            msg += $"\r\n金钱：{castle.Golds}";
            msg += $"\r\n粮草：{castle.Commissariat}";
            msg += $"\r\n建筑：{string.Join(",", castle.Buildings.Select(p => $"{p.Key}(lv.{p.Value})"))}";

            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "KindomStormAI_RenameCastle",
            Command = "重命名城堡",
            Description = "重命名自己的城堡",
            Syntax = "[城堡名]",
            Tag = CmdTagEnum.王国风云,
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = false)]
        public bool RenameCastle(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            castle.CastleName = name;
            castle.Update();

            MsgSender.PushMsg(MsgDTO, "重命名成功！", true);
            return true;
        }
    }
}
