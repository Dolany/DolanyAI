using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai.SingleCommand.Dice
{
    [AI(Name = "骰娘",
        Description = "AI for dice.",
        Enable = true,
        PriorityLevel = 5,
        NeedManulOpen = true)]
    public class DiceAI : AIBase
    {
        private readonly int DiceCountMaxLimit = Configger.Instance.AIConfig.DiceCountMaxLimit;
        private readonly int DiceSizeMaxLimit = Configger.Instance.AIConfig.DiceSizeMaxLimit;

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

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            if (!setting.HasFunction("骰娘"))
            {
                return false;
            }

            var format = MsgDTO.Command;

            var model = ParseDice(format);
            if (model == null || model.Count > DiceCountMaxLimit || model.Size > DiceSizeMaxLimit)
            {
                return false;
            }

            var result = ConsoleDice(model);
            SendResult(MsgDTO, result);
            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = AIAttr.Name,
                Command = "DiceOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });
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
                var value = CommonUtil.RandInt(model.Size) + 1;
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
            var rand = CommonUtil.RandInt(100) + 1;

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
