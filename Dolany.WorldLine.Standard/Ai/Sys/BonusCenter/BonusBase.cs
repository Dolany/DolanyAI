using Dolany.Ai.Common.Models;

namespace Dolany.WorldLine.Standard.Ai.Sys.BonusCenter
{
    public abstract class BonusBase
    {
        public abstract string Code { get; }

        public abstract bool SendBonus(MsgInformationEx MsgDTO);
    }
}
