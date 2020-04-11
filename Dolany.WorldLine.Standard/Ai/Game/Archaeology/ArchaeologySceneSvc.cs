using System.Collections.Generic;
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
    }

    public class ArchaeologySceneModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<ArchaeologySubSceneModel> SubScenes { get; set; }

        public int Exp { get; set; }

        public int Level { get; set; }
    }

    public class ArchaeologySubSceneModel
    {
        public string Name { get; set; }

        public string ArchType { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}
