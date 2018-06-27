using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AILib.Entities;

namespace AILib
{
    [AI(
        Name = "RepeatorAI",
        Description = "AI for Repeating Random words.",
        IsAvailable = true,
        PriorityLevel = 1
        )]
    public class RepeatorAI : AIBase
    {
        private long RepeatLimit = 30;

        private long CurCount = 0;

        private int SleepTime = 1000;

        private object lockObj = new object();

        public RepeatorAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
        }

        public override void Work()
        {
        }

        public override bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            base.OnGroupMsgReceived(MsgDTO);

            if (!IsAvailable(MsgDTO.fromGroup))
            {
                return false;
            }

            if (AIMgr.AllAvailableCommands.Select(p => p.Command).Contains(MsgDTO.command))
            {
                return false;
            }

            CurCount++;
            if (CurCount >= RepeatLimit)
            {
                CurCount %= RepeatLimit;
                Repeat(MsgDTO);
                return true;
            }

            return false;
        }

        [EnterCommand(
            Command = "复读机禁用",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.群主,
            Description = "禁用复读机功能，禁用后将不会在本群产生计数和进行复读",
            Syntax = "",
            Tag = "复读机",
            SyntaxChecker = "Empty"
            )]
        public void Forbidden(GroupMsgDTO MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.fromGroup, false);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "复读机禁用成功！"
            });
        }

        [EnterCommand(
            Command = "复读机启用",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.群主,
            Description = "重新启用复读机功能",
            Syntax = "",
            Tag = "复读机",
            SyntaxChecker = "Empty"
            )]
        public void Unforbidden(GroupMsgDTO MsgDTO, object[] param)
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
            if (query.IsNullOrEmpty())
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
            if (query.IsNullOrEmpty())
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
                Msg = MsgDTO.fullMsg
            });
        }

        [EnterCommand(
            Command = "设定复读频率",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "设定复读功能的频率，即多少次计数后进行复读",
            Syntax = "[复读频率]",
            Tag = "复读机",
            SyntaxChecker = "Long"
            )]
        public void SetRepeatLimit(PrivateMsgDTO MsgDTO, object[] param)
        {
            RepeatLimit = (long)param[0];
        }
    }
}