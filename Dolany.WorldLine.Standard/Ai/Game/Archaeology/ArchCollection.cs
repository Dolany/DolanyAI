using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchCollection : DbBaseEntity
    {
        public long QQNum { get; set; }

        public List<ArchItemColleModel> ItemColles { get; set; } = new List<ArchItemColleModel>();

        public List<string> SpecialColles { get; set; } = new List<string>();

        public Dictionary<string, int> Collectables { get; set; } = new Dictionary<string, int>();

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
    }

    public class ArchItemColleModel
    {
        public string Name { get; set; }

        public Dictionary<string, int> Segments { get; set; } = new Dictionary<string, int>();
    }
}
