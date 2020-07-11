using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.Ai.Game.SignIn;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class SignInAI : SignInBaseAI
    {
        public CastleBuildingSvc CastleBuildingSvc { get; set; }

        protected override void Sign(MsgInformationEx MsgDTO)
        {
            var sign = SignInSuccessiveRecord.Sign(MsgDTO.FromGroup, MsgDTO.FromQQ);
            var goldsGen = Math.Min(sign.SuccessiveDays * 5, 50);

            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            castle.Golds += goldsGen;
            castle.Update();

            var indexNo = SignInGroupInfo.GetAndUpdate(MsgDTO.FromGroup);

            var sessionID = MsgSender.StartSession(MsgDTO);

            var msg = $"签到成功！你已连续签到 {sign.SuccessiveDays}天，获得 {goldsGen.CurencyFormat()}！\r\n本群签到排名：【No.{indexNo}】";
            MsgSender.PushMsg(sessionID, msg);

            Recruit(MsgDTO, sessionID);
            MsgSender.ConfirmSend(sessionID);
        }

        private void Recruit(MsgInformation MsgDTO, string sessionID)
        {
            var castle = KindomCastle.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            
            CollectTaxes(castle, sessionID);
            RecruitCommissariat(castle, sessionID);

            castle.Update();
        }

        private void CollectTaxes(KindomCastle castle, string sessionID)
        {
            var townLevel = castle.SafeBuildings["城镇"];
            var townModel = CastleBuildingSvc["城镇"];
            var GoldGen = townModel.CollecRate * townLevel;

            castle.Golds += GoldGen;
            MsgSender.PushMsg(sessionID, $"成功征税{GoldGen}！当前金钱剩余{castle.Golds}");
        }

        private void RecruitCommissariat(KindomCastle castle, string sessionID)
        {
            var GranaryLevel = castle.SafeBuildings["粮仓"];
            var GranaryModel = CastleBuildingSvc["粮仓"];
            var CommissariatGen = GranaryModel.CollecRate * GranaryLevel;

            castle.Commissariat += CommissariatGen;
            MsgSender.PushMsg(sessionID, $"成功征粮{CommissariatGen}！当前粮食剩余{castle.Commissariat}");
        }
    }
}
