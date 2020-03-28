using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.KindomStorm
{
    public class KindomStormWorldLine : IWorldLine
    {
        public override string Name { get; set; } = "王国风云";
        public override CmdTag CmdTagTree { get; set; } = new CmdTag()
        {
            Tag = CmdTagEnum.Root,
            SubTags = new []
            {
                new CmdTag()
                {
                    Tag = CmdTagEnum.超管
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.系统命令
                }, 
                new CmdTag()
                {
                    Tag = CmdTagEnum.王国风云
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.商店功能
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.人品功能
                }
            }
        };
    }
}
