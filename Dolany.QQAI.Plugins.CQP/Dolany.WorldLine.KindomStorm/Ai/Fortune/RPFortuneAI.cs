using System.Text;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Sqlite.Model;
using Newtonsoft.Json;

namespace Dolany.WorldLine.KindomStorm.Ai.Fortune
{
    public class RPFortuneAI : AIBase
    {
        public override string AIName { get; set; } = "人品";
        public override string Description { get; set; } = "Ai for RP.";
        public override AIPriority PriorityLevel { get; } = AIPriority.Normal;
        public override CmdTagEnum DefaultTag { get; } = CmdTagEnum.人品功能;

        [EnterCommand(ID = "RPFortuneAI_RandomFortune",
            Command = "人品鉴定",
            Description = "获取每天人品鉴定",
            IsPrivateAvailable = true)]
        public bool RandomFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var response = PersonCacheRecord.Get(MsgDTO.FromQQ, "RandomFortune");

            if (string.IsNullOrEmpty(response.Value))
            {
                var randFor = GetRandomFortune();
                var rf = new RandomFortuneCache {QQNum = MsgDTO.FromQQ, FortuneValue = randFor, BlessName = string.Empty, BlessValue = 0};
                ShowRandFortune(MsgDTO, rf);

                response.Value = JsonConvert.SerializeObject(rf);
                response.ExpiryTime = CommonUtil.UntilTommorow();
                response.Update();
            }
            else
            {
                ShowRandFortune(MsgDTO, JsonConvert.DeserializeObject<RandomFortuneCache>(response.Value));
            }
            return true;
        }

        private static int GetRandomFortune()
        {
            return Rander.RandInt(101);
        }

        private static void ShowRandFortune(MsgInformationEx MsgDTO, RandomFortuneCache rf)
        {
            var msg = "你今天的运势是：" + rf.FortuneValue + "%\r\n";

            var builder = new StringBuilder();
            builder.Append(msg);

            for (var i = 0; i < rf.FortuneValue; i++)
            {
                builder.Append("|");
            }

            msg = builder.ToString();

            MsgSender.PushMsg(MsgDTO, msg, true);
        }
    }
}
