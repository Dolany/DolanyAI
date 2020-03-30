using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core
{
    public class CrossWorldAiSvc : IDependency
    {
        public GroupSettingSvc GroupSettingSvc { get; set; }

        public IWorldLine[] AllWorlds { get; set; }

        public IEnumerable<AIBase> CrossWorldAis => AutofacSvc.LoadAllInstanceFromClass<AIBase>(GetType().Assembly);

        public IWorldLine DefaultWorldLine => AllWorlds.First(w => w.IsDefault);

        public IWorldLine this[string worldLineName] => AllWorlds.FirstOrDefault(w => w.Name == worldLineName) ?? DefaultWorldLine;

        public IWorldLine this[long GroupNum] => GroupNum == 0 ? DefaultWorldLine : this[GroupSettingSvc[GroupNum].WorldLine];

        public void InitWorlds(IEnumerable<Assembly> assemblies)
        {
            AllWorlds = AutofacSvc.LoadAllInstanceFromClass<IWorldLine>(assemblies).ToArray();
        }

        public string JudgeWorldLine(long groupNum)
        {
            if (groupNum == 0)
            {
                return DefaultWorldLine.Name;
            }

            var group = GroupSettingSvc[groupNum];
            if (group == null)
            {
                return DefaultWorldLine.Name;
            }

            return string.IsNullOrEmpty(group.WorldLine) ? DefaultWorldLine.Name : group.WorldLine;
        }
    }
}
