using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core
{
    public class CrossWorldAiMgr
    {
        public IWorldLine[] AllWorlds { get; set; }

        public List<AIBase> CrossWorldAis => CommonUtil.LoadAllInstanceFromClass<AIBase>(GetType().Assembly);
    }
}
