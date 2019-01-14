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
        private readonly int DiceCountMaxLimit = int.Parse(CommonUtil.GetConfig(nameof(DiceCountMaxLimit)));
        private readonly int DiceSizeMaxLimit = int.Parse(CommonUtil.GetConfig(nameof(DiceSizeMaxLimit)));

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
            if (model == null ||
                model.count > DiceCountMaxLimit ||
                model.size > DiceSizeMaxLimit)
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

            if (!int.TryParse(strs1[1], out var modify) ||
                modify <= 0)
            {
                return msg;
            }
            model.modify = modify;
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
                if (!int.TryParse(strs[0], out var size) ||
                    size <= 0)
                {
                    return null;
                }
                model.size = size;
                model.count = 1;

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
                model.size = size;
                model.count = count;

                return model;
            }
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
                var value = RandInt(model.size) + 1;
                result.result.Add(value);
            }

            return result;
        }

        private static void SendResult(MsgInformationEx MsgDTO, DiceResultModel ResultModel)
        {
            var sum = ResultModel.result.Sum(p => p);
            var sb = string.Join("+", ResultModel.result.Select(p => p.ToString()));

            if (ResultModel.modify != 0)
            {
                sum += ResultModel.modify;
                sb += $"+{ResultModel.modify}";
            }

            sb += $"={sum}";

            MsgSender.Instance.PushMsg(MsgDTO, sb, true);
        }
    }

    public class DiceModel
    {
        public int count { get; set; }
        public int size { get; set; }
        public int modify { get; set; }
    }

    public class DiceResultModel
    {
        public List<int> result { get; set; }
        public int modify { get; set; }
    }
}
