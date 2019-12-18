using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;

namespace Dolany.Ai.WSMidware.CommandResolver
{
    public class WithdrawMessageCmd : ICmdResovler
    {
        public CommandType CommandType { get; } = CommandType.WithdrawMessage;
        public void Resolve(MsgCommand command)
        {
            var model = new Dictionary<string, object>
            {
                {"id", command.Id },
                {"method", "withdrawMessage"},
                {
                    "params", new Dictionary<string, object>()
                    {
                        {"group", command.ToGroup.ToString()},
                        {"msgid", command.Msg}
                    }
                }
            };
            WSMgr.Instance.Send(command.BindAi, model);
        }

        public void CallBack(WaitingModel model, QQEventModel eventModel)
        {
        }
    }
}
