using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AILib.Entities;

namespace AILib
{
    [AI(Name = "RepeatorAI",
        Description = "AI for Repeating Random words.",
        IsAvailable = true,
        PriorityLevel = 0)]
    public class RepeatorAI : AIBase
    {
        private int RepeatLimit = 30;

        private int CurCount = 0;

        private int SleepTime = 1000;

        private object lockObj = new object();

        public RepeatorAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
        }

        public override void Work()
        {
        }

        public override void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            base.OnGroupMsgReceived(MsgDTO);

            if (MsgDTO.msg.Contains("CQ:"))
            {
                return;
            }

            if (!IsAvailable(MsgDTO.fromGroup))
            {
                return;
            }

            if (AIMgr.AllAvailableCommands.Contains(MsgDTO.command))
            {
                return;
            }

            CurCount++;
            if (CurCount >= RepeatLimit)
            {
                CurCount %= RepeatLimit;
                Repeat(MsgDTO);
            }
        }

        [EnterCommand(Command = "复读机禁用", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.群主)]
        public void Forbidden(GroupMsgDTO MsgDTO)
        {
            ForbiddenStateChange(MsgDTO.fromGroup, false);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "复读机禁用成功！"
            });
        }

        [EnterCommand(Command = "复读机启用", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.群主)]
        public void Unforbidden(GroupMsgDTO MsgDTO)
        {
            ForbiddenStateChange(MsgDTO.fromGroup, true);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "复读机启用成功！"
            });
        }

        private void ForbiddenStateChange(long fromGroup, bool state)
        {
            var query = DbMgr.Query<RepeaterAvailableEntity>(r => r.GroupNumber == fromGroup);
            if (query == null || query.Count() == 0)
            {
                DbMgr.Insert(new RepeaterAvailableEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNumber = fromGroup,
                    Content = state.ToString()
                });
                return;
            }

            var ra = query.FirstOrDefault();
            ra.Content = state.ToString();
            DbMgr.Update(ra);
        }

        private bool IsAvailable(long GroupNum)
        {
            var query = DbMgr.Query<RepeaterAvailableEntity>();
            if (query == null || query.Count() == 0)
            {
                return true;
            }

            foreach (var r in query)
            {
                if (r.GroupNumber == GroupNum && !bool.Parse(r.Content))
                {
                    return false;
                }
            }

            return true;
        }

        private void Repeat(GroupMsgDTO MsgDTO)
        {
            Thread.Sleep(SleepTime);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = MsgDTO.command + " " + MsgDTO.msg
            });
        }

        [EnterCommand(Command = "设定复读频率", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void SetRepeatLimit(PrivateMsgDTO MsgDTO)
        {
            int limit;
            if (!int.TryParse(MsgDTO.msg, out limit) || limit <= 0)
            {
                return;
            }

            RepeatLimit = limit;
        }
    }
}