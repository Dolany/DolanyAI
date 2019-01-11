using System;
using System.Linq;
using System.Threading;

namespace Dolany.Ai.Core.Ai.Repeater
{
    using System.Collections.Generic;

    using Dolany.Ai.Core;
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;
    using Dolany.Ai.Core.SyntaxChecker;
    using Dolany.Database.Ai;

    [AI(
        Name = nameof(RepeatorAI),
        Description = "AI for Repeating Random words.",
        IsAvailable = true,
        PriorityLevel = 1)]
    public class RepeatorAI : AIBase
    {
        private const long RepeatLimit = 30;

        private long CurCount;

        private const int SleepTime = 2000;

        private readonly List<long> InactiveGroups = new List<long>();

        private readonly object List_lock = new object();

        public override void Work()
        {
            Load();
        }

        private void Load()
        {
            using (var db = new AIDatabase())
            {
                var query = db.RepeaterAvailable.Where(p => !p.Available);
                lock (List_lock)
                {
                    this.InactiveGroups.AddRange(query.Select(p => p.GroupNumber));
                }
            }
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
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
            if (CurCount < RepeatLimit)
            {
                return false;
            }
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
            IsPrivateAvailable = false)]
        public void Forbidden(MsgInformationEx MsgDTO, object[] param)
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
            IsPrivateAvailable = false)]
        public void Unforbidden(MsgInformationEx MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, true);

            MsgSender.Instance.PushMsg(MsgDTO, "复读机启用成功！");
        }

        private void ForbiddenStateChange(long fromGroup, bool state)
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

            lock (this.List_lock)
            {
                if (state)
                {
                    this.InactiveGroups.Add(fromGroup);
                }
                else
                {
                    this.InactiveGroups.RemoveAll(p => p == fromGroup);
                }
            }
        }

        private bool IsAvailable(long GroupNum)
        {
            lock (this.List_lock)
            {
                return !InactiveGroups.Contains(GroupNum);
            }
        }

        private static void Repeat(MsgInformationEx MsgDTO)
        {
            Thread.Sleep(SleepTime);

            MsgSender.Instance.PushMsg(MsgDTO, MsgDTO.FullMsg);
        }
    }
}
