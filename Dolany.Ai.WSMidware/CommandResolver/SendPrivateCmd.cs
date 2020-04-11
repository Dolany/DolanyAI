using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;

namespace Dolany.Ai.WSMidware.CommandResolver
{
    public class SendPrivateCmd : ICmdResovler
    {
        public CommandType CommandType { get; } = CommandType.SendPrivate;
        private static WSMgr WSMgr => AutofacSvc.Resolve<WSMgr>();
        public void Resolve(MsgCommand command)
        {
            var model = new Dictionary<string, object>
            {
                {"id", command.Id },
                {"method", "sendMessage"},
                {
                    "params", new Dictionary<string, object>()
                    {
                        {"type", 1},
                        {"qq", command.ToQQ.ToString()},
                        {"content", command.Msg}
                    }
                }
            };
            WSMgr.Send(command.BindAi, model);
        }

        public void CallBack(WaitingModel model, QQEventModel eventModel)
        {
        }
    }
}
