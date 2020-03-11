using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core
{
    public class CrossWorldAiSvc : IDependency
    {
        public IWorldLine[] AllWorlds { get; set; }

        public List<AIBase> CrossWorldAis => CommonUtil.LoadAllInstanceFromClass<AIBase>(GetType().Assembly);

        public IWorldLine DefaultWorldLine => AllWorlds.First(w => w.IsDefault);
    }
}
