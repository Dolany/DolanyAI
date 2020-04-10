using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Doremi
{
    public class DoremiWorldLine : IWorldLine
    {
        public override string Name { get; set; } = "Doremi";
        public override CmdTag CmdTagTree { get; set; } = new CmdTag()
        {
            Tag = CmdTagEnum.Root,
            SubTags = new[]
            {
                new CmdTag()
                {
                    Tag = CmdTagEnum.系统命令
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.群管理
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.打招呼功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.修仙功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.语音功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.娱乐功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.运势功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.语录功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.寻缘功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.商店功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.签到功能
                }
            }
        };
    }
}
