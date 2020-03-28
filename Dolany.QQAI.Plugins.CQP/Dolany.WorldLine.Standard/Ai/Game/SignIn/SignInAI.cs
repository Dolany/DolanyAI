using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.SignIn
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
            if (sign.SuccessiveDays % 10 == 0)
            {
                var cache = PersonCacheRecord.Get(MsgDTO.FromQQ, "抽奖");
                cache.Value = !string.IsNullOrEmpty(cache.Value) && int.TryParse(cache.Value, out var times) ? (times + 1).ToString() : 1.ToString();
                cache.Update();

                msg += "\r\n恭喜你获得一次抽奖机会，快去试试吧（当日有效！）";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
        }
    }
}
