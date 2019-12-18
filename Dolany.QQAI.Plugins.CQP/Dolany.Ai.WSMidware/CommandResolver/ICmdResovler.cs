using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;

namespace Dolany.Ai.WSMidware.CommandResolver
{
    public interface ICmdResovler
    {
        CommandType CommandType { get; }
        void Resolve(MsgCommand command);
        void CallBack(WaitingModel model, QQEventModel eventModel);
    }
}
