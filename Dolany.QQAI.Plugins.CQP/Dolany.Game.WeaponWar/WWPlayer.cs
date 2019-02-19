using System;
using System.Collections.Generic;
using System.Linq;
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

        //弹药仓库
        public IList<BulletStoreModel> BulletStore { get; set; } = new List<BulletStoreModel>();

        //最大特殊材料种类数量
        public int MaxSMCount { get; set; }

        //最大药品携带量
        public int MaxMedicineCount { get; set; }

        //装备仓库
        public IList<ArmModel> ArmStore { get; set; } = new List<ArmModel>();

        //装备的武器
        public WeaponArmModel[] Weapons { get; set; } = {new WeaponArmModel(), new WeaponArmModel(), new WeaponArmModel()};

        //装备的防具
        public ArmModel[] Shields { get; set; } = {new ArmModel(), new ArmModel(), new ArmModel()};

        //特殊材料
        public Dictionary<string, int> SpecialMDic { get; set; } = new Dictionary<string, int>();
        //药品
        public Dictionary<string, DateTime> MedicineDic { get; set; } = new Dictionary<string, DateTime>();

        private int CurWeight =>
            Weapons.Sum(w =>
            {
                if (w.Weapon == null)
                {
                    return 0;
                }

                var weapon = WWWeaponHelper.Instance.FindWeapon(w.Weapon.Code);
                return weapon.Weight;
            });

        public static WWPlayer GetPlayer(long QQNum)
        {
            var player = MongoService<WWPlayer>.Get(p => p.QQNum == QQNum).FirstOrDefault();
            if (player == null)
            {
                player = new WWPlayer() {QQNum = QQNum};
                MongoService<WWPlayer>.Insert(player);
            }

            return player;
        }

        private int LocateWeapon(string code)
        {
            for (var i = 0; i < Weapons.Length; i++)
            {
                if (Weapons[i].Weapon != null && Weapons[i].Weapon.Code == code)
                {
                    return i;
                }
            }

            return -1;
        }

        private int LocateArmStore(string code)
        {
            for (var i = 0; i < ArmStore.Count; i++)
            {
                if (ArmStore[i].Code == code)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Update()
        {
            for (var i = 0; i < 3; i++)
            {
                if (Weapons[i] != null && Weapons[i].Weapon.HP == 0)
                {
                    Weapons[i].Weapon = null;
                }
                if (Shields[i] != null && Shields[i].HP == 0)
                {
                    Shields[i] = null;
                }
            }

            SpecialMDic.Remove(m => m == 0);
            MedicineDic.Remove(m => m.ToLocalTime() < DateTime.Now);
            BulletStore.Remove(b => b.Count == 0);

            MongoService<WWPlayer>.Update(this);
        }

        public bool CheckWeaponAvailable(int weaponNo, int bulletCount, out string msg)
        {
            if (Weapons[weaponNo].Weapon == null)
            {
                msg = "该武器位没有装备武器！";
                return false;
            }

            if (Weapons[weaponNo].Weapon.CDTime.ToLocalTime() > DateTime.Now)
            {
                msg = "该武器尚在冷却中！";
                return false;
            }

            if (Weapons[weaponNo].Bullet == null || Weapons[weaponNo].Bullet.Count == 0)
            {
                msg = "该武器位弹药不足！";
                return false;
            }

            var weapon = WWWeaponHelper.Instance.FindWeapon(Weapons[weaponNo].Weapon.Code);
            if (!weapon.BulletKind.Contains(Weapons[weaponNo].Bullet.Code))
            {
                msg = $"武器{weapon.Code}无法使用弹药{Weapons[weaponNo].Bullet.Code}！";
                return false;
            }

            if (Weapons[weaponNo].Bullet.Count < bulletCount)
            {
                msg = "该武器位弹药不足！";
                return false;
            }

            msg = string.Empty;
            return true;
        }

        public bool ArmWeapon(string code, int no, out string msg)
        {
            var weapLoc = LocateWeapon(code);
            if (weapLoc >= 0)
            {
                if (weapLoc + 1 == no)
                {
                    msg = $"武器{code}已经装备在武器位{no}上了！";
                    return false;
                }

                var sourceW = Weapons[no - 1].Weapon;
                var aimW = Weapons[weapLoc].Weapon;
                Weapons[no - 1].Weapon = sourceW;
                Weapons[weapLoc].Weapon = aimW;

                msg = "交换成功！";
                if (!IsWeaponBulletMatch(no))
                {
                    msg += $"(警告！武器位{no}上的武器无法使用该位置上的弹药！)";
                }
                if (!IsWeaponBulletMatch(weapLoc))
                {
                    msg += $"(警告！武器位{weapLoc + 1}上的武器无法使用该位置上的弹药！)";
                }
                return true;
            }

            weapLoc = LocateArmStore(code);
            if (weapLoc < 0)
            {
                msg = "你没有该武器！";
                return false;
            }

            var originW = Weapons[no].Weapon;
            Weapons[no].Weapon = ArmStore[weapLoc];
            ArmStore.RemoveAt(weapLoc);
            if (originW != null)
            {
                if (CurWeight - WWWeaponHelper.Instance.FindWeapon(originW.Code).Weight + WWWeaponHelper.Instance.FindWeapon(code).Weight > MaxWeight)
                {
                    msg = "超出负重上限！";
                    return false;
                }

                msg = "替换成功！";
                ArmStore.Add(originW);
            }
            else
            {
                if (CurWeight + WWWeaponHelper.Instance.FindWeapon(code).Weight > MaxWeight)
                {
                    msg = "超出负重上限！";
                    return false;
                }

                msg = "装备成功！";
            }
            if (!IsWeaponBulletMatch(no))
            {
                msg += $"(警告！武器位{no}上的武器无法使用该位置上的弹药！)";
            }

            return true;
        }

        private bool IsWeaponBulletMatch(int no)
        {
            var waModel = Weapons[no];
            if (waModel.Weapon == null || waModel.Bullet == null)
            {
                return true;
            }

            var wModel = WWWeaponHelper.Instance.FindWeapon(waModel.Weapon.Code);
            return wModel.BulletKind.Contains(waModel.Bullet.Code);
        }
    }

    public class ArmModel
    {
        public string Code { get; set; }

        //当前耐久
        public int HP { get; set; }

        //冷却
        public DateTime CDTime { get; set; }
    }

    public class WeaponArmModel
    {
        public ArmModel Weapon { get; set; }

        public BulletStoreModel Bullet { get; set; }
    }

    public enum ArmType
    {
        Weapon,
        Shield
    }

    public class BulletStoreModel
    {
        public string Code { get; set; }

        public int Count { get;set; }
    }
}
