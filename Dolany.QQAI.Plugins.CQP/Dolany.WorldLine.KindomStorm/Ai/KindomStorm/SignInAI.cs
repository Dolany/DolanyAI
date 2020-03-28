using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.Ai.Game.SignIn;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class SignInAI : SignInBaseAI
    {
        protected override void Sign(MsgInformationEx MsgDTO)
        {
            var sign = SignInSuccessiveRecord.Sign(MsgDTO.FromGroup, MsgDTO.FromQQ);
            var goldsGen = Math.Min(sign.SuccessiveDays * 5, 50);

            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            castle.Golds += goldsGen;
            castle.Update();

            var indexNo = SignInGroupInfo.GetAndUpdate(MsgDTO.FromGroup);

            var msg = $"签到成功！你已连续签到 {sign.SuccessiveDays}天，获得 {goldsGen.CurencyFormat()}！\r\n本群签到排名：【No.{indexNo}】";
            MsgSender.PushMsg(MsgDTO, msg, true);
        }
    }
}
