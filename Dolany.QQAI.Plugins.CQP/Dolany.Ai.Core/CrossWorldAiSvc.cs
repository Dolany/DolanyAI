using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core
{
    public class CrossWorldAiSvc : IDependency
    {
        public GroupSettingSvc GroupSettingSvc { get; set; }

        public IWorldLine[] AllWorlds { get; set; }

        public List<AIBase> CrossWorldAis => CommonUtil.LoadAllInstanceFromClass<AIBase>(GetType().Assembly);

        public IWorldLine DefaultWorldLine => AllWorlds.First(w => w.IsDefault);

        public IWorldLine this[string worldLineName] => AllWorlds.FirstOrDefault(w => w.Name == worldLineName) ?? DefaultWorldLine;

        public IWorldLine this[long GroupNum] => GroupNum == 0 ? DefaultWorldLine : this[GroupSettingSvc[GroupNum].WorldLine];
    }
}
