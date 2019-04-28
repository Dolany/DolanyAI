using System.Collections.Generic;
using System.Linq;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Cooking
{
    public class CookKitchen : DbBaseEntity
    {
        public long QQNum { get; set; }

        public List<CookTool> Tools { get; set; } = new List<CookTool>();

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
    }

    public class CookTool
    {
        public string Name { get; set; }

        public CookToolTypeEnum ToolType { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public bool IsInUse { get; set; }
    }

    public enum CookToolTypeEnum
    {
        炒锅 = 1,
        菜刀 = 2,
        砧板 = 3
    }
}
