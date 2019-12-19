using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.OnlineStore.Vip
{
    public class VipMgr
    {
        public static VipMgr Instance { get; } = new VipMgr();

        private VipMgr(){}

        public static void Charge(ChargeModel model)
        {
            var diamonds = (int) (Math.Round(model.Amount, 2) * 100);
            var chargeRec = new VipChargeRecord()
            {
                QQNum = model.QQNum,
                ChargeAmount = model.Amount,
                ChargeTime = DateTime.Now,
                DiamondAmount = diamonds,
                Message = model.Message,
                OrderID = model.OrderID
            };
            chargeRec.Insert();

            var osPerson = OSPerson.GetPerson(model.QQNum);
            osPerson.Diamonds += diamonds;
            osPerson.Update();

            MsgSender.PushMsg(0, model.QQNum, $"恭喜充值成功！当前余额：{osPerson.Diamonds}{Emoji.钻石}", model.BindAi);
        }
    }
}
