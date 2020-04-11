using Dolany.Ai.WSMidware.Models;

namespace Dolany.Ai.WSMidware.MessageResolver
{
    public interface IMsgResolver
    {
        string MsgEvent { get; }
        void Resolver(string bindAi, QQEventModel model);
    }
}
