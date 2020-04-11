using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.WorldLine.Doremi.OnlineStore;
using Dolany.WorldLine.Doremi.Xiuxian;

namespace Dolany.WorldLine.Doremi.Ai.Game.Xiuxian
{
    public class MsgCounterAI : AIBase
    {
        public override string AIName { get; set; } = "修仙计数器";
        public override string Description { get; set; } = "AI for Msg Count for Xiuxian.";
        public override AIPriority PriorityLevel { get; } = AIPriority.High;
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.修仙功能;

        private List<long> EnablePersons = new List<long>();
        private const int DujieQACount = 3;

        public DujieSvc DujieSvc { get; set; }
        public LevelSvc LevelSvc { get; set; }

        public override void Initialization()
        {
            EnablePersons = MsgCounterSvc.GetAllEnabledPersons();
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private || !EnablePersons.Contains(MsgDTO.FromQQ))
            {
                return false;
            }

            MsgCounterSvc.Cache(MsgDTO.FromQQ);
            return false;
        }

        [EnterCommand(ID = "MsgCounterAI_EnableXiuxian",
            Command = "开启修仙模式",
            Description = "开启修仙模式，每日发言将会获取经验值，经验值可用于升级")]
        public bool EnableXiuxian(MsgInformationEx MsgDTO, object[] param)
        {
            if (EnablePersons.Contains(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你已经开启了修仙模式！", true);
                return false;
            }

            EnablePersons.Add(MsgDTO.FromQQ);
            MsgCounterSvc.PersonEnable(MsgDTO.FromQQ);

            MsgSender.PushMsg(MsgDTO, "开启成功！");
            return true;
        }

        [EnterCommand(ID = "MsgCounterAI_Disable",
            Command = "关闭修仙模式",
            Description = "关闭修仙模式，将不会获得新的经验值，原有的数据还将保留")]
        public bool Disable(MsgInformationEx MsgDTO, object[] param)
        {
            if (!EnablePersons.Contains(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你尚未开启修仙模式！", true);
                return false;
            }

            EnablePersons.Remove(MsgDTO.FromQQ);
            MsgCounterSvc.PersonDisable(MsgDTO.FromQQ);

            MsgSender.PushMsg(MsgDTO, "关闭成功！");
            return true;
        }

        [EnterCommand(ID = "MsgCounterAI_Upgrade",
            Command = "渡劫",
            Description = "消耗经验值升级")]
        public bool Upgrade(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson_Doremi.GetPerson(MsgDTO.FromQQ);
            var level = LevelSvc.GetByLevel(osPerson.Level);
            var exp = MsgCounterSvc.Get(MsgDTO.FromQQ);

            if (exp < level.Exp)
            {
                MsgSender.PushMsg(MsgDTO, "你没有足够的经验值升级！", true);
                return false;
            }

            if (level.Level == LevelSvc.TopLevel)
            {
                MsgSender.PushMsg(MsgDTO, "你已满级，无需渡劫！", true);
                return false;
            }

            MsgSender.PushMsg(MsgDTO, "渡劫开始！你需要回答对全部问题才能成功渡劫！");

            var qas = DujieSvc.RandQAs(DujieQACount);
            int i;
            for (i = 0; i < DujieQACount; i++)
            {
                var randAs = qas[i].RandAs;
                var msg = $"题目（{i + 1}/{DujieQACount}）：\r\n" +
                          $"{qas[i].Q}\r\n" +
                          $"{string.Join("\r\n", randAs.Select((p, idx) => $"{idx + 1}:{p}"))}";
                var i1 = i;
                var info = WaiterSvc.WaitForInformation(MsgDTO, msg, information => information.FromGroup == MsgDTO.FromGroup &&
                                                                                          information.FromQQ == MsgDTO.FromQQ &&
                                                                                          int.TryParse(information.Msg, out var idx) &&
                                                                                          idx > 0 && idx <= qas[i1].A.Length, 10);
                if (info == null)
                {
                    MsgSender.PushMsg(MsgDTO, "回答超时！");
                    break;
                }

                var aidx = int.Parse(info.Msg) - 1;
                if (!qas[i].IsCorrect(randAs[aidx]))
                {
                    MsgSender.PushMsg(MsgDTO, "回答错误！");
                    break;
                }

                MsgSender.PushMsg(MsgDTO, "回答正确！");
            }

            MsgCounterSvc.Consume(MsgDTO.FromQQ, level.Exp);
            if (i != DujieQACount)
            {
                MsgSender.PushMsg(MsgDTO, "渡劫失败，请重新来过！", true);
                return true;
            }

            osPerson.Level++;
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "升级成功！");
            return true;
        }

        [EnterCommand(ID = "MsgCounterAI_Exchange",
            Command = "兑换金币",
            Description = "使用经验值兑换金币",
            DailyLimit = 3)]
        public bool Exchange(MsgInformationEx MsgDTO, object[] param)
        {
            var exp = MsgCounterSvc.Get(MsgDTO.FromQQ);
            var golds = exp / 2;

            if (golds == 0)
            {
                MsgSender.PushMsg(MsgDTO, "你没有足够的经验值兑换！");
                return false;
            }

            var osPerson = OSPerson_Doremi.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds += (int)golds;
            osPerson.Update();
            MsgCounterSvc.Consume(MsgDTO.FromQQ, exp);

            MsgSender.PushMsg(MsgDTO, $"兑换成功！你使用 {exp} 点经验值兑换了 {golds}金币，你当前拥有 {osPerson.Golds}金币！");
            return true;
        }
    }
}
