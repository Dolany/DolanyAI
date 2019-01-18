namespace Dolany.Ai.Core.Ai.SingleCommand.Dice
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Model;

    using static Common.Utility;

    [AI(
        Name = nameof(DiceAI),
        Description = "AI for dice.",
        IsAvailable = true,
        PriorityLevel = 5)]
    public class DiceAI : AIBase
    {
        private readonly int DiceCountMaxLimit = int.Parse(Configger.Instance["DiceCountMaxLimit"]);
        private readonly int DiceSizeMaxLimit = int.Parse(Configger.Instance["DiceSizeMaxLimit"]);

        public DiceAI()
        {
            RuntimeLogger.Log("DiceAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            var format = MsgDTO.Command;

            var model = ParseDice(format);
            if (model == null || model.Count > DiceCountMaxLimit || model.Size > DiceSizeMaxLimit)
            {
                return false;
            }

            var result = ConsoleDice(model);
            SendResult(MsgDTO, result);
            return true;
        }

        private static DiceModel ParseDice(string msg)
        {
            var model = new DiceModel();

            if (msg.Contains("+"))
            {
                msg = CheckModify(msg, model);
            }

            return !msg.Contains("d") ? null : CheckD(msg, model);
        }

        private static string CheckModify(string msg, DiceModel model)
        {
            if (msg.Count(p => p == '+') > 1)
            {
                return msg;
            }

            var strs1 = msg.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs1.Length != 2)
            {
                return msg;
            }

            if (!int.TryParse(strs1[1], out var modify) || modify <= 0)
            {
                return msg;
            }
            model.Modify = modify;
            return strs1[0];
        }

        private static DiceModel CheckD(string msg, DiceModel model)
        {
            if (msg.Count(p => p == 'd') > 1)
            {
                return null;
            }

            var strs = msg.Split(new[] { 'd' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 1 && strs.Length != 2)
            {
                return null;
            }

            if (strs.Length == 1)
            {
                if (!int.TryParse(strs[0], out var size) || size <= 0)
                {
                    return null;
                }
                model.Size = size;
                model.Count = 1;

                return model;
            }
            else
            {
                if (!int.TryParse(strs[0], out var count) ||
                    !int.TryParse(strs[1], out var size) ||
                    count <= 0 ||
                    size <= 0)
                {
                    return null;
                }
                model.Size = size;
                model.Count = count;

                return model;
            }
        }

        private static DiceResultModel ConsoleDice(DiceModel model)
        {
            var result = new DiceResultModel
            {
                Modify = model.Modify,
                Result = new List<int>()
            };

            for (var i = 0; i < model.Count; i++)
            {
                var value = RandInt(model.Size) + 1;
                result.Result.Add(value);
            }

            return result;
        }

        private static void SendResult(MsgInformationEx MsgDTO, DiceResultModel ResultModel)
        {
            var sum = ResultModel.Result.Sum(p => p);
            var sb = string.Join("+", ResultModel.Result.Select(p => p.ToString()));

            if (ResultModel.Modify != 0)
            {
                sum += ResultModel.Modify;
                sb += $"+{ResultModel.Modify}";
            }

            sb += $"={sum}";

            MsgSender.Instance.PushMsg(MsgDTO, sb, true);
        }
    }

    public class DiceModel
    {
        public int Count { get; set; }
        public int Size { get; set; }
        public int Modify { get; set; }
    }

    public class DiceResultModel
    {
        public List<int> Result { get; set; }
        public int Modify { get; set; }
    }
}
