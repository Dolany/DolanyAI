using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;
using Newtonsoft.Json;

namespace Dolany.Ai.WSMidware.MessageResolver
{
    public class GroupMemberIncreaseMsg : IMsgResolver
    {
        public string MsgEvent { get; } = "groupMemberIncrease";
        public void Resolver(string bindAi, QQEventModel model)
        {
            var changeModel = new GroupMemberChangeModel()
            {
                QQNum = long.TryParse(model.Params.Qq, out var qqNum) ? qqNum : 0,
                GroupNum = long.TryParse(model.Params.Group, out var groupNum) ? groupNum : 0,
                Type = 0,
                Operator = long.TryParse(model.Params.Operator, out var op) ? op : 0
            };

            var info = new MsgInformation()
            {
                BindAi = bindAi,
                Information = InformationType.ReceiveMoney,
                Msg = JsonConvert.SerializeObject(changeModel),
                FromQQ = changeModel.QQNum,
                FromGroup = changeModel.GroupNum
            };
            WSMgr.Instance.PublishInformation(info);
        }
    }
}
