using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Game.WeaponWar
{
    public class WWPlayer : BaseEntity
    {
        public long QQNum { get; set; }

        public int MaxHP { get; set; }

        public int HP { get; set; }

        //最大负重
        public int MaxWeight { get; set; }

        //最大装备仓库数量
        public int MaxArmStoreCount { get; set; }

        //装备仓库
        public IList<ArmModel> ArmStore { get; set; }

        public IList<ArmModel> Weapons { get; set; } = new List<ArmModel>() {null, null, null};

        public IList<ArmModel> Shields { get; set; } = new List<ArmModel>() {null, null, null};

        //特殊材料
        public Dictionary<string, int> SpecialMDic { get; set; }
        //药品
        public Dictionary<string, DateTime> MedicineDic { get; set; }

        public void Update()
        {
            for (var i = 0; i < 3; i++)
            {
                if (Weapons[i] != null && Weapons[i].HP == 0)
                {
                    Weapons[i] = null;
                }
                if (Shields[i] != null && Shields[i].HP == 0)
                {
                    Shields[i] = null;
                }
            }

            SpecialMDic.Remove(m => m == 0);
            MedicineDic.Remove(m => m.ToLocalTime() < DateTime.Now);

            MongoService<WWPlayer>.Update(this);
        }
    }

    public class ArmModel
    {
        public string Name { get; set; }

        public int Level { get; set; }

        //最大耐久
        public int MaxHP { get; set; }

        //当前耐久
        public int HP { get; set; }

        //冷却
        public DateTime CDTime { get; set; }

        //负重
        public int Weight { get; set; }
    }

    public enum ArmType
    {
        Weapon,
        Shield
    }
}
