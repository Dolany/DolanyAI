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

    public class SEAreaModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Scenes { get; set; }

        public string DefaultScene { get; set; }

        public string[] NeighbourAreas { get; set; }
    }

    public class SESceneModel
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 场景类型：Town/Boss/Wild
        /// </summary>
        public string Type { get; set; }

        public string[] AccessableScenes { get; set; }

        #region Wild

        public string[] Monsters { get; set; }

        public int MonsterRate { get; set; }

        /// <summary>
        /// 空闲时的文字提示
        /// </summary>
        public string IdleWords { get; set; }

        #endregion

        #region Boss

        public string BossName { get; set; }

        /// <summary>
        /// Boss重生时间间隔（小时）
        /// </summary>
        public int BossRebornInterval { get; set; }

        public string HiddenBoss { get; set; }

        public int HiddenBossRate { get; set; }

        #endregion
    }
}
