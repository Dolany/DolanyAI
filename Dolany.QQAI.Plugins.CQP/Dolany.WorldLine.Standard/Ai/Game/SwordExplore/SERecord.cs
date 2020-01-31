using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.SwordExplore
{
    public class SERecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int MaxHP { get; set; }

        public int CurHP { get; set; }

        public int MaxEndurance { get; set; }

        public int CurEndurance { get; set; }

        public string CurScene { get; set; }

        public List<string> ClearAreas { get; set; } = new List<string>();

        public Dictionary<string, int> Skills { get; set; } = new Dictionary<string, int>();

        public SEAttribute[] Attrs { get; set; }

        public static SERecord Get(long QQNum)
        {
            return MongoService<SERecord>.GetOnly(p => p.QQNum == QQNum);
        }

        public void Update()
        {
            MongoService<SERecord>.Update(this);
        }

        public int RollAttr(string attrType)
        {
            if (Attrs.IsNullOrEmpty() || Attrs.All(a => a.AttrType != attrType))
            {
                return 0;
            }

            var attr = Attrs.First(a => a.AttrType == attrType);
            return Rander.RandInt(attr.Max + 1 - attr.Min) + attr.Min;
        }
    }

    public class SEAttribute
    {
        public string AttrType { get; set; }

        public int Max { get; set; }

        public int Min { get; set; }
    }
}
