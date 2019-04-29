using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Alchemy
{
    public class AlchemyKitchen : DbBaseEntity
    {
        public long QQNum { get; set; }

        public List<AlchemyTool> Tools { get; set; } = new List<AlchemyTool>();

        public List<AlchemyFood> Foods { get; set; } = new List<AlchemyFood>();

        public Dictionary<string, int> Flavorings { get; set; } = new Dictionary<string, int>();

        public AlchemyTool this[AlchemyToolTypeEnum type]
        {
            get { return Tools.FirstOrDefault(p => p.ToolType == type && p.IsInUse); }
        }

        public void ChangeTool(string name)
        {
            var tool = Tools.First(t => t.Name == name);
            this[tool.ToolType].IsInUse = false;
            tool.IsInUse = true;
        }

        public static AlchemyKitchen GetKitchen(long QQNum)
        {
            var record = MongoService<AlchemyKitchen>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new AlchemyKitchen(){QQNum = QQNum};
            MongoService<AlchemyKitchen>.Insert(record);
            return record;
        }

        public void Update()
        {
            Flavorings.Remove(p => p == 0);

            MongoService<AlchemyKitchen>.Update(this);
        }
    }

    public class AlchemyFood
    {
        public string Name { get; set; }

        public int Sugar { get; set; }

        public int Health { get; set; }

        public int SAN { get; set; }

        public string[] Additions { get; set; }
    }

    public class AlchemyTool
    {
        public string Name { get; set; }

        public AlchemyToolTypeEnum ToolType { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public bool IsInUse { get; set; }

        public string[] Additions { get; set; }
    }

    public enum AlchemyToolTypeEnum
    {
        坩埚 = 1,
        研钵 = 2,
        点金棒 = 3
    }
}
