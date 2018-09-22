using System;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Threading;
using Dolany.Ice.Ai.DolanyAI.Utils;

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
        private const long RepeatLimit = 30;

        private long CurCount;

        private const int SleepTime = 3000;

        public override void Work()
        {
        }

        public override bool OnMsgReceived(ReceivedMsgDTO MsgDTO)
        {
            base.OnMsgReceived(MsgDTO);

            if (!IsAvailable(MsgDTO.FromGroup))
            {
                return false;
            }
            var atChecker = new AtChecker();
            if (atChecker.Check(MsgDTO.FullMsg, out _))
            {
                return false;
            }

            if (AIMgr.Instance.AllAvailableGroupCommands.Select(p => p.Command).Contains(MsgDTO.Command))
            {
                return false;
            }

            CurCount++;
            if (CurCount < RepeatLimit) return false;
            CurCount %= RepeatLimit;
            Repeat(MsgDTO);
            return true;
        }

        [EnterCommand(
            Command = "复读机禁用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "禁用复读机功能，禁用后将不会在本群产生计数和进行复读",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false
            )]
        public void Forbidden(ReceivedMsgDTO MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, false);

            MsgSender.Instance.PushMsg(MsgDTO, "复读机禁用成功！");
        }

        [EnterCommand(
            Command = "复读机启用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "重新启用复读机功能",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false
            )]
        public void Unforbidden(ReceivedMsgDTO MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, true);

            MsgSender.Instance.PushMsg(MsgDTO, "复读机启用成功！");
        }

        private static void ForbiddenStateChange(long fromGroup, bool state)
        {
            using (var db = new AIDatabase())
            {
                var query = db.RepeaterAvailable.Where(r => r.GroupNumber == fromGroup);
                if (query.IsNullOrEmpty())
                {
                    db.RepeaterAvailable.Add(new RepeaterAvailable
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupNumber = fromGroup,
                        Available = state
                    });
                }
                else
                {
                    var ra = query.FirstOrDefault();
                    if (ra != null)
                    {
                        ra.Available = state;
                    }
                }

                db.SaveChanges();
            }
        }

        private static bool IsAvailable(long GroupNum)
        {
            using (var db = new AIDatabase())
            {
                var query = db.RepeaterAvailable;
                if (query.IsNullOrEmpty())
                {
                    return true;
                }

                return Enumerable.All(query, r => r.GroupNumber != GroupNum ||
                                                  r.Available);
            }
        }

        private static void Repeat(ReceivedMsgDTO MsgDTO)
        {
            Thread.Sleep(SleepTime);

            MsgSender.Instance.PushMsg(MsgDTO, MsgDTO.FullMsg);
        }
    }
}