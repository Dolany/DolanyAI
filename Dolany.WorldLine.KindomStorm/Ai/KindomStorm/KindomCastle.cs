using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class KindomCastle : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int Level { get; set; } = 1;

        public string CharactorName { get; set; }

        public string CastleName { get; set; }

        public int Golds { get; set; }

        /// <summary>
        /// 粮草
        /// </summary>
        public int Commissariat { get; set; }

        public Dictionary<string, int> Buildings { get; set; } = new Dictionary<string, int>();

        public SafeDictionary<string, int> SafeBuildings => Buildings.ToSafe();

        public int SoldierMaxVolume => Level * 100;

        public int LvlUpNeedGold => Level * 50;

        public static KindomCastle Get(long GroupNum, long QQNum)
        {
            var castle = MongoService<KindomCastle>.GetOnly(p => p.QQNum == QQNum);
            if (castle != null)
            {
                return castle;
            }

            var group = AutofacSvc.Resolve<GroupSettingSvc>()[GroupNum];
            var info = APIEx.GetQQInfo(QQNum, group.BindAi);
            castle = new KindomCastle()
            {
                QQNum = QQNum, CharactorName = info.nick, CastleName = $"{info.nick}的城堡", Buildings = CastleBuildingSvc.Buildings.ToDictionary(p => p.Name, p => 1)
            };
            MongoService<KindomCastle>.Insert(castle);
            return castle;
        }

        public static IEnumerable<KindomCastle> GetAll()
        {
            return MongoService<KindomCastle>.Get();
        }

        public void Update()
        {
            MongoService<KindomCastle>.Update(this);
        }

        public void UpgradeCastle(int lvlUp = 1)
        {
            Level += lvlUp;
            Golds -= LvlUpNeedGold;
        }

        public void UpgradeBuilding(string buildingName, int lvlUp = 1)
        {
            var safeB = SafeBuildings;
            safeB[buildingName] += lvlUp;
            Buildings = safeB.Data;
        }
    }
}
