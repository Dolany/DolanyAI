using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Sys.BonusCenter
{
    public abstract class BonusBase
    {
        public abstract string Code { get; }

        public abstract bool IsExpiried { get; }

        public abstract bool SendBonus(MsgInformationEx MsgDTO);
    }
}
