using System.Linq;
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
        public override int PriorityLevel { get; set; } = 100;

        [EnterCommand(ID = "WorldLineMgrAi_SwitchWorldLine",
            Command = "切换世界线",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "切换世界线",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "系统命令",
            IsPrivateAvailable = false)]
        public bool SwitchWorldLine(MsgInformationEx MsgDTO, object[] param)
        {
            var option = Waiter.Instance.WaitForOptions(MsgDTO.FromGroup, MsgDTO.FromQQ, "请选择需要切换的世界线：",
                CrossWorldAiMgr.Instance.AllWorlds.Select(w => w.Name).ToArray(), MsgDTO.BindAi);
            if (option < 0)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var worldLine = CrossWorldAiMgr.Instance.AllWorlds[option];
            var group = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            group.WorldLine = worldLine.Name;
            group.Update();

            MsgSender.PushMsg(MsgDTO, "世界线切换成功！");
            return true;
        }
    }
}
