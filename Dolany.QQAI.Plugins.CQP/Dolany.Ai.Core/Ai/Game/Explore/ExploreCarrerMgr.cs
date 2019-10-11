using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Explore
{
    public class ExploreCarrerMgr
    {
        public static ExploreCarrerMgr Instance { get; } = new ExploreCarrerMgr();

        private List<ExploreCarrerModel> Carrers;

        private ExploreCarrerMgr()
        {
            Carrers = CommonUtil.ReadJsonData_NamedList<ExploreCarrerModel>("ExploreCarrerData");
        }
    }

    public class ExploreCarrerModel : INamedJsonModel
    {
        public string Name { get; set; }

        public Dictionary<int ,int> LevelDic { get; set; }
    }
}
