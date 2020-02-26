using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;

namespace Dolany.Ai.WSMidware.CommandResolver
{
    public class SendGroupCmd : ICmdResovler
    {
        public CommandType CommandType { get; } = CommandType.SendGroup;
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
                        {"type", 2},
                        {"group", command.ToGroup.ToString()},
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
