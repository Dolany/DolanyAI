using System;
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
            if (base.OnGroupMsgReceived(MsgDTO))
            {
                return true;
            }

            var query = DbMgr.Query<DiceSettingRecordEntity>(p => p.Content == MsgDTO.Command &&
                                                                  p.FromGroup == MsgDTO.FromGroup);
            var entities = query as DiceSettingRecordEntity[] ?? query.ToArray();
            var format = entities.IsNullOrEmpty() ? MsgDTO.Command : entities.First().SourceFormat;

            var model = ParseDice(format);
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

            var strs1 = msg.Split('+');
            if (strs1.Length != 2)
            {
                return msg;
            }

            if (!int.TryParse(strs1[1], out var modify) || modify <= 0)
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

            var strs = msg.Split('d');
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

        [GroupEnterCommand(
            Command = "save",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "保存自定义骰子格式",
            Syntax = "[标准格式] [自定义命令名称]",
            Tag = "骰子功能",
            SyntaxChecker = "TwoWords"
        )]
        public void SaveFormatAs(GroupMsgDTO MsgDTO, object[] param)
        {
            var sourceFormat = param[0] as string;
            var savedFormat = param[1] as string;

            if (ParseDice(sourceFormat) == null)
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = "源格式错误，请使用类似3d20[+][3]的格式！"
                });
                return;
            }

            var query = DbMgr.Query<DiceSettingRecordEntity>(p => p.SourceFormat == sourceFormat &&
                                                                  p.FromGroup == MsgDTO.FromGroup);
            var entities = query as DiceSettingRecordEntity[] ?? query.ToArray();
            if (entities.IsNullOrEmpty())
            {
                DbMgr.Insert(new DiceSettingRecordEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    FromGroup = MsgDTO.FromGroup,
                    SourceFormat = sourceFormat,
                    Content = savedFormat,
                    UpdateTime = DateTime.Now
                });
            }
            else
            {
                var setting = entities.First();
                setting.Content = savedFormat;
                setting.UpdateTime = DateTime.Now;

                DbMgr.Update(setting);
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "保存成功！"
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