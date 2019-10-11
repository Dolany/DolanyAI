using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Explore
{
    public class ExploreSceneMgr
    {
        public static ExploreSceneMgr Instance { get; } = new ExploreSceneMgr();

        private readonly List<ExploreSceneModel> SceneList;

        private ExploreSceneMgr()
        {
            SceneList = CommonUtil.ReadJsonData_NamedList<ExploreSceneModel>("ExploreSceneData");
        }

        public List<ExploreSceneModel> RandScene(int level)
        {
            var levelScenes = SceneList.Where(s => s.Level >= level - 1 && s.Level <= level + 1);
            return Rander.RandSort(levelScenes.ToArray()).Take(3).ToList();
        }
    }

    public class ExploreSceneModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public string Description { get; set; }

        public string PicPath { get; set; }

        public string[] AdjoinAreas { get; set; }

        public List<ExploreEncounterModel> Encounters { get; set; }
    }

    public class ExploreEncounterModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PicPath { get; set; }

        public string Kind { get; set; }

        public int Rate { get; set; }

        public Dictionary<string, object> InfoDic { get; set; }
    }
}
