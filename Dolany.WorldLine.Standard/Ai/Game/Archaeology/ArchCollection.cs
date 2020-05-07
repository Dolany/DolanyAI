using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    /// <summary>
    /// 考古收藏记录
    /// </summary>
    public class ArchCollection : DbBaseEntity
    {
        public long QQNum { get; set; }

        /// <summary>
        /// 考古收藏品碎片(普通收集品)
        /// </summary>
        public List<ArchItemColleModel> ItemColles { get; set; } = new List<ArchItemColleModel>();

        /// <summary>
        /// 特殊收藏品(唯一)
        /// </summary>
        public List<string> SpecialColles { get; set; } = new List<string>();

        /// <summary>
        /// 考古收藏品(合成后)
        /// </summary>
        public Dictionary<string, int> Collectables { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// 考古地图碎片(5个碎片可以拼接成一个考古地图)
        /// </summary>
        public int MapSegments { get; set; }

        public static ArchCollection Get(long QQNum)
        {
            var rec = MongoService<ArchCollection>.GetOnly(p => p.QQNum == QQNum);
            if (rec != null)
            {
                return rec;
            }

            rec = new ArchCollection(){QQNum = QQNum};
            MongoService<ArchCollection>.Insert(rec);
            return rec;
        }

        public void AddCollections(Dictionary<string, int> dic)
        {
            var colle = Collectables.ToSafe();
            foreach (var (name, count) in dic)
            {
                colle[name] += count;
            }

            Collectables = colle.Data;
        }

        public void Update()
        {
            MongoService<ArchCollection>.Update(this);
        }
    }

    /// <summary>
    /// 考古收藏品碎片(每种考古收藏品有十种不同的碎片)
    /// </summary>
    public class ArchItemColleModel
    {
        public string Name { get; set; }

        public Dictionary<string, int> Segments { get; set; } = new Dictionary<string, int>();
    }
}
