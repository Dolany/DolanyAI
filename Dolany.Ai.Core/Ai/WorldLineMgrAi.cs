﻿using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai
{
    public class WorldLineMgrAi : AIBase
    {
        public override string AIName { get; set; } = "世界线管理器";
        public override string Description { get; set; } = "Ai for world line management.";
        public override AIPriority PriorityLevel { get;} = AIPriority.Monitor;
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.系统命令;

        public CrossWorldAiSvc CrossWorldAiSvc { get; set; }

        [EnterCommand(ID = "WorldLineMgrAi_SwitchWorldLine",
            Command = "切换世界线",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "切换世界线")]
        public bool SwitchWorldLine(MsgInformationEx MsgDTO, object[] param)
        {
            var option = WaiterSvc.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, "请选择需要切换的世界线：",
                CrossWorldAiSvc.AllWorlds.Select(w => w.Name).ToArray(), MsgDTO.BindAi);
            if (option < 0)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var worldLine = CrossWorldAiSvc.AllWorlds[option];
            var group = GroupSettingSvc[MsgDTO.FromGroup];
            group.WorldLine = worldLine.Name;
            group.Update();

            MsgSender.PushMsg(MsgDTO, "世界线切换成功！");
            return true;
        }
    }
}
