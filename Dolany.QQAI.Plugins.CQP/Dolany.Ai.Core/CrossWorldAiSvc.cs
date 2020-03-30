using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core
{
    public class CrossWorldAiSvc : IDependency
    {
        public GroupSettingSvc GroupSettingSvc { get; set; }

        public IWorldLine[] AllWorlds { get; set; }

        public IEnumerable<AIBase> CrossWorldAis => CommonUtil.LoadAllInstanceFromClass<AIBase>(GetType().Assembly);

        public IWorldLine DefaultWorldLine => AllWorlds.First(w => w.IsDefault);

        public IWorldLine this[string worldLineName] => AllWorlds.FirstOrDefault(w => w.Name == worldLineName) ?? DefaultWorldLine;

        public IWorldLine this[long GroupNum] => GroupNum == 0 ? DefaultWorldLine : this[GroupSettingSvc[GroupNum].WorldLine];

        public void InitWorlds(IEnumerable<Assembly> assemblies)
        {
            var baseType = typeof(IWorldLine);
            AllWorlds = assemblies.SelectMany(p => p.GetTypes().Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract))
                .Where(type => AutofacSvc.Container.IsRegistered(type)).Select(type => AutofacSvc.Resolve(type) as IWorldLine).Where(d => d != null)
                .ToArray();
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
