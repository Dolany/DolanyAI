using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Threading;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(RepeatorAI),
        Description = "AI for Repeating Random words.",
        IsAvailable = true,
        PriorityLevel = 1
        )]
    public class RepeatorAI : AIBase
    {
        private long RepeatLimit = 30;

        private long CurCount = 0;

        private int SleepTime = 3000;

        private object lockObj = new object();

        public RepeatorAI()
            : base()
        {
            //this.ComposePartsSelf();
        }

        public override void Work()
        {
        }

        public override bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            base.OnGroupMsgReceived(MsgDTO);

            if (!IsAvailable(MsgDTO.FromGroup))
            {
                return false;
            }
            if (MsgDTO.FullMsg.Contains("CQ:at"))
            {
                return false;
            }

            if (AIMgr.Instance.AllAvailableGroupCommands.Select(p => p.Command).Contains(MsgDTO.Command))
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

        [GroupEnterCommand(
            Command = "复读机禁用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "禁用复读机功能，禁用后将不会在本群产生计数和进行复读",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty"
            )]
        public void Forbidden(GroupMsgDTO MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, false);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "复读机禁用成功！"
            });
        }

        [GroupEnterCommand(
            Command = "复读机启用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "重新启用复读机功能",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty"
            )]
        public void Unforbidden(GroupMsgDTO MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, true);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "复读机启用成功！"
            });
        }

        private static void ForbiddenStateChange(long fromGroup, bool state)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.RepeaterAvailable.Where(r => r.GroupNumber == fromGroup);
                if (query.IsNullOrEmpty())
                {
                    db.RepeaterAvailable.Add(new RepeaterAvailable()
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupNumber = fromGroup,
                        Available = state
                    });
                }
                else
                {
                    var ra = query.FirstOrDefault();
                    ra.Available = state;
                }

                db.SaveChanges();
            }
        }

        private static bool IsAvailable(long GroupNum)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.RepeaterAvailable;
                if (query.IsNullOrEmpty())
                {
                    return true;
                }

                foreach (var r in query)
                {
                    if (r.GroupNumber == GroupNum && !r.Available)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void Repeat(GroupMsgDTO MsgDTO)
        {
            Thread.Sleep(SleepTime);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = MsgDTO.FullMsg
            });
        }

        [PrivateEnterCommand(
            Command = "设定复读频率",
            Description = "设定复读功能的频率，即多少次计数后进行复读",
            Syntax = "[复读频率]",
            Tag = "复读机功能",
            SyntaxChecker = "Long"
            )]
        public void SetRepeatLimit(PrivateMsgDTO MsgDTO, object[] param)
        {
            RepeatLimit = (long)param[0];
        }
    }
}