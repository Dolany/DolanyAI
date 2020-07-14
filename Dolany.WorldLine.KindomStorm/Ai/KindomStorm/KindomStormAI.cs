using System;
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
            var groups = SoldierGroup.Get(MsgDTO.FromQQ);

            var sessionID = MsgSender.StartSession(MsgDTO);
            MsgSender.PushMsg(sessionID, castle.CastleName);
            MsgSender.PushMsg(sessionID, $"城堡等级：{Utility.LevelEmoji(castle.Level)}");
            MsgSender.PushMsg(sessionID, $"金钱：{castle.Golds}");
            MsgSender.PushMsg(sessionID, $"粮草：{castle.Commissariat}");
            MsgSender.PushMsg(sessionID, $"建筑：{string.Join(",", castle.Buildings.Select(p => $"{p.Key}(lv.{p.Value})"))}");
            MsgSender.PushMsg(sessionID, $"军队：{groups.Count}支");
            MsgSender.PushMsg(sessionID, $"士兵：{groups.Sum(g => g.Count)}人");

            MsgSender.ConfirmSend(sessionID);
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

        [EnterCommand(ID = "KindomStormAI_Recruit",
            Command = "招募",
            Description = "招募军队",
            DailyLimit = 2)]
        public bool Recruit(MsgInformationEx MsgDTO, object[] param)
        {
            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);

            var soldierGroups = SoldierGroup.Get(MsgDTO.FromQQ);
            var curTotal = soldierGroups.Sum(g => g.Count);

            var remain = Math.Max(castle.SoldierMaxVolume - curTotal, 0);
            var msg = $"你当前共拥有士兵 {curTotal}人，还可招募 {remain}人";
            if (remain == 0)
            {
                msg += "\r\n无法招募新的士兵！";
                MsgSender.PushMsg(MsgDTO, msg, true);
                return false;
            }

            var golds = castle.Golds;
            msg += $"\r\n你当前拥有金钱 {golds}，最多招募等量的士兵";
            if (golds == 0)
            {
                msg += "\r\n无法招募新的士兵！";
                MsgSender.PushMsg(MsgDTO, msg, true);
                return false;
            }

            var maxRecruit = Math.Min(remain, golds);
            var num = WaiterSvc.WaitForNum(MsgDTO.FromGroup, MsgDTO.FromQQ, $"请输入招募士兵的数量！(1~{maxRecruit})", p => p >= 1 && p <= maxRecruit, MsgDTO.BindAi);
            if (num <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var group = new SoldierGroup()
            {
                Owner = MsgDTO.FromQQ,
                Count = num,
                State = SoldierState.Working,
                Name = $"第{GroupNamingRec.GetNextNo(MsgDTO.FromQQ)}军"
            };
            group.Insert();

            castle.Golds -= num;
            castle.Update();

            MsgSender.PushMsg(MsgDTO, "招募成功！");
            return true;
        }
    }
}
