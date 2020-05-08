using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchaeologySceneSvc : IDataMgr, IDependency
    {
        private List<ArchaeologySceneModel> Scenes;

        public void RefreshData()
        {
            Scenes = CommonUtil.ReadJsonData_NamedList<ArchaeologySceneModel>("Standard/Arch/ArchSceneData");
        }

        public IEnumerable<ArchaeologySceneModel> RandScenes(int count)
        {
            return Rander.RandSort(Scenes.ToArray()).Take(count).ToList();
        }

        public List<ArchaeologySceneModel> GetLevelScene(List<string> sceneNames, Dictionary<string, int> levelDic)
        {
            var safeDic = levelDic.ToSafe();
            var scenes = Scenes.Where(p => sceneNames.Contains(p.Name)).ToList();
            scenes = scenes.Select(p =>
            {
                var level = safeDic[p.Name];
                p.Level = level == 0 ? 1 : level;
                return p;
            }).ToList();
            return scenes;
        }
    }

    public class ArchaeologySceneModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<ArchaeologySubSceneModel> SubScenes { get; set; }

        public int Level { get; set; } = 1;
    }

    public class ArchaeologySubSceneModel
    {
        public string Name { get; set; }

        public string ArchType { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}
