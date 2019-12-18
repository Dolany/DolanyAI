using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;
using Newtonsoft.Json;

namespace Dolany.Ai.WSMidware.CommandResolver
{
    public class GetGroupMemberInfoCmd : ICmdResovler
    {
        public CommandType CommandType { get; } = CommandType.GetGroupMemberInfo;
        public void Resolve(MsgCommand command)
        {
            WSMgr.Instance.WaitingDic.TryAdd(command.Id, new WaitingModel() {BindAi = command.BindAi, Command = command.Command, RelationId = command.Id});

            var model = new Dictionary<string, object>()
            {
                {"id", command.Id },
                {"method", "getGroupMemberList" },
                {"params", new Dictionary<string, object>()
                {
                    {"group", command.Msg }
                } }
            };
            WSMgr.Instance.Send(command.BindAi, model);
        }

        public void CallBack(WaitingModel model, QQEventModel eventModel)
        {
            var info = new MsgInformation()
            {
                Information = InformationType.CommandBack,
                RelationId = model.RelationId,
                Msg = JsonConvert.SerializeObject(eventModel.Result)
            };
            WSMgr.Instance.PublishInformation(info);
        }
    }
}
