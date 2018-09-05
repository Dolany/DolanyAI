using System.Collections.Generic;
using System.Linq;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(DiceAI),
        Description = "AI for dice.",
        IsAvailable = true,
        PriorityLevel = 5
    )]
    public class DiceAI : AIBase
    {
        public DiceAI()
        {
            RuntimeLogger.Log("DiceAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            var model = ParseDice(MsgDTO.Command);
            if (model == null)
            {
                return false;
            }

            var result = ConsoleDice(model);
            SendResult(MsgDTO, result);
            return true;
        }

        private static DiceModel ParseDice(string msg)
        {
            // TODO
            return null;
        }

        private static DiceResultModel ConsoleDice(DiceModel model)
        {
            var result = new DiceResultModel
            {
                modify = model.modify,
                result = new List<int>()
            };

            for (var i = 0; i < model.count; i++)
            {
                var value = Utility.RandInt(model.size) + 1;
                result.result.Add(value);
            }

            return result;
        }

        private static void SendResult(GroupMsgDTO MsgDTO, DiceResultModel ResultModel)
        {
            var sum = ResultModel.result.Sum(p => p);
            var sb = string.Join("+", ResultModel.result.Select(p => p.ToString()));

            if (ResultModel.modify != 0)
            {
                sum += ResultModel.modify;
                sb += $"+{ResultModel.modify}";
            }

            sb += $"={sum}";

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = $"{CodeApi.Code_At(MsgDTO.FromQQ)} {sb}"
            });
        }
    }

    public class DiceModel
    {
        public int count { get; set; }
        public int size { get; set; }
        public int modify { get; set; } = 0;
    }

    public class DiceResultModel
    {
        public List<int> result { get; set; }
        public int modify { get; set; }
    }
}