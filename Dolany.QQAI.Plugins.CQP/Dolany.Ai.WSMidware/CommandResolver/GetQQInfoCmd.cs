using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;
using Newtonsoft.Json;

namespace Dolany.Ai.WSMidware.CommandResolver
{
    public class GetQQInfoCmd : ICmdResovler
    {
        public CommandType CommandType { get; } = CommandType.GetQQInfo;
        public void Resolve(MsgCommand command)
        {
            WSMgr.Instance.WaitingDic.TryAdd(command.Id, new WaitingModel() {BindAi = command.BindAi, Command = command.Command, RelationId = command.Id});

            var model = new Dictionary<string, object>()
            {
                {"id", command.Id },
                {"method", "getQQInfo" },
                {"params", new Dictionary<string, object>()
                {
                    {"qq", command.ToQQ.ToString() }
                } }
            };
            WSMgr.Instance.Send(command.BindAi, model);
        }

        public void CallBack(WaitingModel model, QQEventModel eventModel)
        {
            try
            {
                dynamic result = eventModel.Result["result"];
                var buddy = result["buddy"];
                var info_list = buddy["info_list"];

                var info = new MsgInformation()
                {
                    Information = InformationType.CommandBack,
                    RelationId = model.RelationId,
                    Msg = JsonConvert.SerializeObject(info_list[0])
                };
                WSMgr.PublishInformation(info);
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
        }
    }
}
