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
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.王国风云;

        public CastleBuildingSvc CastleBuildingSvc { get; set; }

        [EnterCommand(ID = "KindomStormAI_MyCastle",
            Command = "我的城堡",
            Description = "查看自己的城堡的情况")]
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
            SyntaxHint = "[城堡名]",
            SyntaxChecker = "Word")]
        public bool RenameCastle(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            castle.CastleName = name;
            castle.Update();

            MsgSender.PushMsg(MsgDTO, "重命名成功！", true);
            return true;
        }

        [EnterCommand(ID = "KindomStormAI_UpgradeCastle",
            Command = "升级城堡",
            Description = "升级自己的城堡")]
        public bool UpgradeCastle(MsgInformationEx MsgDTO, object[] param)
        {
            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);

            if (castle.Golds < castle.LvlUpNeedGold)
            {
                MsgSender.PushMsg(MsgDTO, $"你没有足够多的金钱升级！（{castle.Golds}/{castle.LvlUpNeedGold}）");
                return false;
            }

            castle.UpgradeCastle();
            castle.Update();

            MsgSender.PushMsg(MsgDTO, $"恭喜你升级成功！当前城堡等级：{castle.Level}");
            return true;
        }

        [EnterCommand(ID = "KindomStormAI_UpgradeBuilding",
            Command = "升级建筑",
            Description = "升级指定的建筑",
            SyntaxHint = "[建筑名]",
            SyntaxChecker = "Word")]
        public bool UpgradeBuilding(MsgInformationEx MsgDTO, object[] param)
        {
            var buildingName = (string) param[0];
            var building = CastleBuildingSvc[buildingName];
            if (building == null)
            {
                MsgSender.PushMsg(MsgDTO, "不认识的建筑呢！");
                return false;
            }

            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            if (!castle.Buildings.ContainsKey(buildingName))
            {
                MsgSender.PushMsg(MsgDTO, "你尚未拥有该建筑！");
                return false;
            }

            var buildingLvl = castle.SafeBuildings[buildingName];
            var goldNeed = building.UpgradeGoldNeed(buildingLvl);

            if (castle.Golds < goldNeed)
            {
                MsgSender.PushMsg(MsgDTO, $"你没有足够多的金钱升级 {buildingName}！({castle.Golds}/{goldNeed})");
                return false;
            }

            castle.Golds -= goldNeed;
            castle.UpgradeBuilding(buildingName);
            castle.Update();

            MsgSender.PushMsg(MsgDTO, $"升级成功！当前{buildingName}的等级为：{castle.SafeBuildings[buildingName]}");
            return true;
        }
    }
}
