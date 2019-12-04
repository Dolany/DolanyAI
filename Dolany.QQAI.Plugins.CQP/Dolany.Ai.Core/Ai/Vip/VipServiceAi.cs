using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class VipServiceAi : AIBase
    {
        public override string AIName { get; set; } = "Vip服务";
        public override string Description { get; set; } = "Ai for vip services.";
        public override int PriorityLevel { get; set; } = 10;


    }
}
