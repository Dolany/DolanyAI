using Dolany.Ai.Common.Models;

namespace Dolany.Ai.Core.Ai.Vip
{
    public interface IVipArmer
    {
        string Name { get; set; }

        string Description { get; set; }

        int DiamondsNeed { get; set; }

        VipArmerLimitInterval LimitInterval { get; set; }

        int LimitCount { get; set; }

        bool Purchase(MsgInformationEx MsgDTO);

        int MaxContains { get; set; }
    }

    public enum VipArmerLimitInterval
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2
    }
}
