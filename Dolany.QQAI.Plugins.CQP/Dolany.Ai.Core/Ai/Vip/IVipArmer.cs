using Dolany.Ai.Common.Models;

namespace Dolany.Ai.Core.Ai.Vip
{
    public interface IVipArmer
    {
        string Name { get; set; }

        string Description { get; set; }

        int DiamondsNeed { get; set; }

        bool Purchase(MsgInformationEx MsgDTO);
    }
}
