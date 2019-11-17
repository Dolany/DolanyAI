using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Pet.Expedition
{
    public class ExpeditionSceneMgr
    {
        public static ExpeditionSceneMgr Instance { get; } = new ExpeditionSceneMgr();

        private List<ExpeditionSceneModel> Scenes;

        private ExpeditionSceneMgr()
        {
            Scenes = CommonUtil.ReadJsonData_NamedList<ExpeditionSceneModel>("Pet/ExpeditionSceneData");
        }
    }

    public class ExpeditionSceneModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Endurance { get; set; }

        public int TimeConsume { get; set; }

        public ExpeditionBonus_Gold GoldBonus { get; set; }

        public ExpeditionBonus_Item ItemBonus { get; set; }

        public ExpeditionBonus_Flavoring FlavoringBonus { get; set; }
    }

    public class ExpeditionBonus_Gold
    {
        public int Min { get; set; }

        public int Max { get; set; }
    }

    public class ExpeditionBonus_Item
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public string[] Options { get; set; }
    }

    public class ExpeditionBonus_Flavoring
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public string[] Options { get; set; }
    }
}
