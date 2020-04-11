using System.Collections.Generic;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;

namespace Dolany.Ai.WSMidware.CommandResolver
{
    public class PraiseCmd : ICmdResovler
    {
        public CommandType CommandType { get; } = CommandType.Praise;
        private static WSMgr WSMgr => AutofacSvc.Resolve<WSMgr>();
        public void Resolve(MsgCommand command)
        {
            var count = int.Parse(command.Msg);
            for (var i = 0; i < count; i++)
            {
                var model = new Dictionary<string, object>()
                {
                    {"method", "givePraise" },
                    {"params", new Dictionary<string, object>()
                    {
                        {"qq", command.ToQQ.ToString() }
                    } }
                };
                WSMgr.Send(command.BindAi, model);

                Thread.Sleep(100);
            }
        }

        public void CallBack(WaitingModel model, QQEventModel eventModel)
        {
        }
    }
}
