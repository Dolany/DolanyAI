using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.Dice
{
    public class DiceAI : AIBase
    {
        public override string AIName { get; set; } = "骰娘";

        public override string Description { get; set; } = "AI for dice.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Low;

        public override bool NeedManualOpeon { get; } = true;

        private const int DiceCountMaxLimit = 200;
        private const int DiceSizeMaxLimit = 2000;

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            var setting = GroupSettingSvc[MsgDTO.FromGroup];
            if (!setting.HasFunction("骰娘"))
            {
                return false;
            }

            if (!ParseFormat(MsgDTO))
            {
                return false;
            }

            AIAnalyzer.AddCommandCount(new CmdRec()
            {
                FunctionalAi = AIName,
                Command = "DiceOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });
            return true;
        }

        private bool ParseFormat(MsgInformationEx MsgDTO)
        {
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
                var value = Rander.RandInt(model.Size) + 1;
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

            MsgSender.PushMsg(MsgDTO, sb, true);
        }

        [EnterCommand(ID = "DiceAI_RD",
            Command = ".rd",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取1-100之间的一个随机数",
            Syntax = "",
            Tag = "骰娘功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool RD(MsgInformationEx MsgDTO, object[] param)
        {
            var rand = Rander.RandInt(100) + 1;

            MsgSender.PushMsg(MsgDTO, $"你掷出了 {rand} 点！", true);
            return true;
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
