using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.WorldLine.Standard.Ai.Game.SignIn;
using MsgSender = Dolany.Ai.Core.Cache.MsgSender;

namespace Dolany.Ai.Doremi.Ai.Game.Shopping
{
    public class SignInAI : SignInBaseAI
    {
        protected override void Sign(MsgInformationEx MsgDTO)
        {
            var sign = SignInSuccessiveRecord.Sign(MsgDTO.FromGroup, MsgDTO.FromQQ);
            var goldsGen = Math.Min(sign.SuccessiveDays * 5, 50);

            OSPerson.GoldIncome(MsgDTO.FromQQ, goldsGen);
            var indexNo = SignInGroupInfo.GetAndUpdate(MsgDTO.FromGroup);

            var msg = $"签到成功！你已连续签到 {sign.SuccessiveDays}天，获得 {goldsGen.CurencyFormat()}！\r\n本群签到排名：【No.{indexNo}】";
            MsgSender.PushMsg(MsgDTO, msg, true);
        }
    }
}
