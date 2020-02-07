using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;
using Newtonsoft.Json;

namespace Dolany.Ai.WSMidware.CommandResolver
{
    public class ConnectionStateCmd : ICmdResovler
    {
        public CommandType CommandType { get; } = CommandType.ConnectionState;
        public void Resolve(MsgCommand command)
        {
            var stateDic = WSMgr.Instance.ClientsDic.ToDictionary(c => c.Key, c => c.Value.IsConnected);
            var info = new MsgInformation()
            {
                Information = InformationType.CommandBack,
                Msg = JsonConvert.SerializeObject(stateDic),
                RelationId = command.Id
            };

            WSMgr.PublishInformation(info);
        }

        public void CallBack(WaitingModel model, QQEventModel eventModel)
        {
        }
    }
}
