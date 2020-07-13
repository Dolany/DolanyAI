using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using MongoDB.Driver;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class SoldierGroupLevelSvc : IDependency
    {
        private IEnumerable<SoldierGroupLevelModel> Models { get; } = new List<SoldierGroupLevelModel>()
        {
            new SoldierGroupLevelModel()
            {
                GroupLevel = SoldierGroupLevel.Corps,
                SoldierCount = 100,
                Name = "军"
            },
            new SoldierGroupLevelModel()
            {
                GroupLevel = SoldierGroupLevel.Division,
                SoldierCount = 10,
                Name = "师"
            },
            new SoldierGroupLevelModel()
            {
                GroupLevel = SoldierGroupLevel.Brigade,
                SoldierCount = 1,
                Name = "旅"
            }
        };

        public SoldierGroupLevelModel this[SoldierGroupLevel level] => Models.FirstOrDefault(m => m.GroupLevel == level);
    }

    public class SoldierGroupLevelModel
    {
        public SoldierGroupLevel GroupLevel { get; set; }

        public int SoldierCount { get; set; }

        public string Name { get; set; }

        public string GetNaming(long QQNum)
        {
            return $"第{SoldierGroupNamingRec.GetNextNo(QQNum, GroupLevel)}{Name}";
        }
    }

    /// <summary>
    /// 军队编制
    /// </summary>
    public enum SoldierGroupLevel
    {
        /// <summary>
        /// 军(100k)
        /// </summary>
        Corps = 1,
        /// <summary>
        /// 师(10k)
        /// </summary>
        Division = 2,
        /// <summary>
        /// 旅(1k)
        /// </summary>
        Brigade = 3
    }

    public class SoldierGroupNamingRec : DbBaseEntity
    {
        public long QQNum { get; set; }

        public SoldierGroupLevel Level { get; set; }

        public int SequenceNo { get; set; }

        public static int GetNextNo(long QQNum, SoldierGroupLevel Level)
        {
            var update = Builders<SoldierGroupNamingRec>.Update.Inc(p => p.SequenceNo, 1);
            var rec = MongoService<SoldierGroupNamingRec>.FindOneAndUpdate(p => p.QQNum == QQNum && p.Level == Level, update, true);
            return rec.SequenceNo;
        }
    }
}
