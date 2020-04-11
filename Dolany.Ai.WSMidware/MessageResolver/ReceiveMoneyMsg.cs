using System;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;
using Newtonsoft.Json;

namespace Dolany.Ai.WSMidware.MessageResolver
{
    public class ReceiveMoneyMsg : IMsgResolver
    {
        public string MsgEvent { get; } = "receiveMoney";
        public void Resolver(string bindAi, QQEventModel model)
        {
            var chargeModel = new ChargeModel()
            {
                QQNum = long.TryParse(model.Params.Qq, out var qqNum) ? qqNum : 0,
                Amount = double.TryParse(model.Params.Amount, out var amount) ? Math.Round(amount, 2) : 0,
                Message = model.Params.Message,
                OrderID = model.Params.Id,
                BindAi = bindAi
            };

            var info = new MsgInformation()
            {
                BindAi = bindAi,
                Information = InformationType.ReceiveMoney,
                Msg = JsonConvert.SerializeObject(chargeModel),
                FromQQ = chargeModel.QQNum
            };
            WSMgr.PublishInformation(info);
        }
    }
}
