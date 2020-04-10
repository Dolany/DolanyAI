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

                }
            }
        };
    }
}
