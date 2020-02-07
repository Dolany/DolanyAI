using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;

namespace Dolany.Ai.WSMidware.MessageResolver
{
    public class MessageMsg : IMsgResolver
    {
        public string MsgEvent { get; } = "message";
        public void Resolver(string bindAi, QQEventModel model)
        {
            if (WSMgr.Instance.AllAis.Any(ai => ai.QQNum == model.Params.Qq))
            {
                return;
            }

            var info = new MsgInformation()
            {
                BindAi = bindAi,
                FromGroup = long.TryParse(model.Params.Group, out var groupNum) ? groupNum : 0,
                FromQQ = long.TryParse(model.Params.Qq, out var qqNum) ? qqNum : 0,
                Information = InformationType.Message,
                Msg = model.Params.Content
            };
            WSMgr.PublishInformation(info);
        }
    }
}
