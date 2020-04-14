using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core
{
    public class CrossWorldAiSvc : IDependency
    {
        public GroupSettingSvc GroupSettingSvc { get; set; }

        public IWorldLine[] AllWorlds { get; set; }

        public IEnumerable<AIBase> CrossWorldAis => AutofacSvc.LoadAllInstanceFromClass<AIBase>(GetType().Assembly);

        public IWorldLine DefaultWorldLine { get; set; }

        public IWorldLine this[string worldLineName] => AllWorlds.FirstOrDefault(w => w.Name == worldLineName) ?? DefaultWorldLine;

        public IWorldLine this[long GroupNum] => GroupNum == 0 ? DefaultWorldLine : this[GroupSettingSvc[GroupNum].WorldLine];

        public void InitWorlds(IEnumerable<Assembly> assemblies)
        {
            AllWorlds = AutofacSvc.LoadAllInstanceFromClass<IWorldLine>(assemblies).ToArray();

            foreach (var worldLine in AllWorlds)
            {
                worldLine.Init();
                worldLine.AIGroup.AddRange(CrossWorldAis);
                worldLine.Load();
            }
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

        public void OnMsgReceived(MsgInformation MsgDTO)
        {
            var worldLine = JudgeWorldLine(MsgDTO.FromGroup);
            var world = AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMsgReceived(MsgDTO);
        }

        public void OnMoneyReceived(ChargeModel model)
        {
            var worldLine = JudgeWorldLine(0);
            var world = AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMoneyReceived(model);
        }

        public void OnGroupMemberChanged(GroupMemberChangeModel model)
        {
            var worldLine = JudgeWorldLine(model.GroupNum);
            var world = AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnGroupMemberChanged(model);
        }
    }
}
