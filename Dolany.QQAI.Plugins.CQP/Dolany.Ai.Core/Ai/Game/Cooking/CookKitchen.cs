using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Cooking
{
    public class CookKitchen : DbBaseEntity
    {
        public long QQNum { get; set; }

        public List<CookTool> Tools { get; set; } = new List<CookTool>();

        public List<CookFood> Foods { get; set; } = new List<CookFood>();

        public Dictionary<string, int> Flavorings { get; set; } = new Dictionary<string, int>();

        public CookTool this[CookToolTypeEnum type]
        {
            get { return Tools.FirstOrDefault(p => p.ToolType == type && p.IsInUse); }
        }

        public void ChangeTool(string name)
        {
            var tool = Tools.First(t => t.Name == name);
            this[tool.ToolType].IsInUse = false;
            tool.IsInUse = true;
        }

        public static CookKitchen GetKitchen(long QQNum)
        {
            var record = MongoService<CookKitchen>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new CookKitchen(){QQNum = QQNum};
            MongoService<CookKitchen>.Insert(record);
            return record;
        }

        public void Update()
        {
            Flavorings.Remove(p => p == 0);

            MongoService<CookKitchen>.Update(this);
        }
    }

    public class CookFood
    {
        public string Name { get; set; }

        public int Sugar { get; set; }

        public int Health { get; set; }

        public int SAN { get; set; }

        public string[] Additions { get; set; }
    }

    public class CookTool
    {
        public string Name { get; set; }

        public CookToolTypeEnum ToolType { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public bool IsInUse { get; set; }

        public string[] Additions { get; set; }
    }

    public enum CookToolTypeEnum
    {
        炒锅 = 1,
        菜刀 = 2,
        砧板 = 3
    }
}
