using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.SwordExplore
{
    public class SEMapMgr
    {
        public static SEMapMgr Instance { get; } = new SEMapMgr();

        private SESceneModel[] Map { get; }

        private List<SEAreaModel> Areas { get; }

        public SESceneModel this[string SceneID] => Map.FirstOrDefault(p => p.ID == SceneID);

        public SESceneModel DefaultScene => this[Areas.First().DefaultScene];

        private SEMapMgr()
        {
            Map = CommonUtil.ReadJsonData<SESceneModel[]>("SE/SEMapData");
            Areas = CommonUtil.ReadJsonData_NamedList<SEAreaModel>("SE/SEAreaData");
        }

        public SEAreaModel FindAreaByScene(string SceneName)
        {
            return Areas.FirstOrDefault(p => p.Scenes.Contains(SceneName));
        }

        public SEAreaModel FindAreaByName(string AreaName)
        {
            return Areas.FirstOrDefault(p => p.Name == AreaName);
        }
    }
}
