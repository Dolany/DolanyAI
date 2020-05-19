using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology.ArchAdv
{
    public class ArchAdvSubSceneSvc : IDependency
    {
        private readonly List<IArchAdvSubScene> SubScenes;

        public ArchAdvSubSceneSvc()
        {
            SubScenes = AutofacSvc.LoadAllInstanceFromInterface<IArchAdvSubScene>();
        }

        public IArchAdvSubScene CreateSubScene(string ArchType, Dictionary<string, object> Data, MsgInformationEx MsgDTO)
        {
            var scene = SubScenes.First(p => p.ArchType == ArchType);
            var type = scene.GetType();
            if (string.IsNullOrEmpty(type.FullName))
            {
                return default;
            }

            var assembly = type.Assembly;

            if (!(assembly.CreateInstance(type.FullName) is IArchAdvSubScene newInstance))
            {
                return default;
            }

            newInstance.MsgDTO = MsgDTO;
            newInstance.Data = Data;
            return newInstance;
        }
    }
}
