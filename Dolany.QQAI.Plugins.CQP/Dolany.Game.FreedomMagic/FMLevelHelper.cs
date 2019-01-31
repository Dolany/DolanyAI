using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Game.FreedomMagic
{
    public class FMLevelModel
    {
        public int Level { get; set; }

        public int FullExp { get; set; }

        public int MaxHP { get; set; }

        public int MaxMP { get; set; }

        public int WaitTime { get; set; }

        public int MaxMagicLevel { get; set; }

        public override string ToString()
        {
            return $"当前等级 {Level}\r" +
                   $"最大HP {MaxHP},最大MP {MaxMP}\r" +
                   $"施法时间 {WaitTime},魔法等级 {MaxMagicLevel}\r" +
                   $"升级需要经验值 {FullExp}";
        }
    }

    public class FMLevelHelper
    {
        public static FMLevelHelper Instance { get; } = new FMLevelHelper();

        private readonly Dictionary<int, FMLevelModel> LevelDic;

        private FMLevelHelper()
        {
            LevelDic = CommonUtil.ReadJsonData<Dictionary<int, FMLevelModel>>("levelData");
            foreach (var (key, value) in LevelDic)
            {
                value.Level = key;
            }
        }

        public FMLevelModel this[int level] => !LevelDic.Keys.Contains(level) ? null : LevelDic[level];
    }
}
