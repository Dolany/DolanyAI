using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Repeater
{
    using Base;
    using Cache;
    using Common;
    using Model;
    using SyntaxChecker;
    using Dolany.Database.Ai;

    [AI(
        Name = nameof(PlusOneAI),
        Description = "AI for Auto Plus One.",
        IsAvailable = true,
        PriorityLevel = 1)]
    public class PlusOneAI : AIBase
    {
        private List<PlusOneCache> Cache { get; } = new List<PlusOneCache>();

        private readonly List<long> InactiveGroups = new List<long>();

        private readonly object List_lock = new object();

        public override void Work()
        {
            Load();
        }

        private void Load()
        {
            var query = MongoService<PlusOneAvailable>.Get(p => !p.Available);
            lock (List_lock)
            {
                this.InactiveGroups.AddRange(query.Select(p => p.GroupNumber));
            }
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (!IsAvailable(MsgDTO.FromGroup))
            {
                return false;
            }
            var checker = new AtChecker();
            if (checker.Check(MsgDTO.FullMsg, out _))
            {
                return false;
            }

            var query = Cache.Where(d => d.GroupNumber == MsgDTO.FromGroup).ToList();
            if (query.IsNullOrEmpty())
            {
                Cache.Add(new PlusOneCache
                {
                    GroupNumber = MsgDTO.FromGroup,
                    IsAlreadyRepeated = false,
                    MsgCache = MsgDTO.FullMsg
                });

                return false;
            }

            Thread.Sleep(2000);
            var groupCache = query.FirstOrDefault();
            Repeat(MsgDTO, groupCache);
            return true;
        }

        private static void Repeat(MsgInformationEx MsgDTO, PlusOneCache groupCache)
        {
            if (groupCache.MsgCache != MsgDTO.FullMsg)
            {
                groupCache.MsgCache = MsgDTO.FullMsg;
                groupCache.IsAlreadyRepeated = false;

                return;
            }
            if (groupCache.IsAlreadyRepeated)
            {
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, MsgDTO.FullMsg);
            groupCache.IsAlreadyRepeated = true;
        }

        [EnterCommand(
            Command = "+1复读禁用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "禁用+1复读功能，禁用后将不会在本群进行+1复读",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public void Forbidden(MsgInformationEx MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, false);

            MsgSender.Instance.PushMsg(MsgDTO, "+1复读禁用成功！");
        }

        [EnterCommand(
            Command = "+1复读启用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "重新启用+1复读功能",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public void Unforbidden(MsgInformationEx MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, true);

            MsgSender.Instance.PushMsg(MsgDTO, "+1复读启用成功！");
        }

        private void ForbiddenStateChange(long fromGroup, bool state)
        {
            var query = MongoService<PlusOneAvailable>.Get(r => r.GroupNumber == fromGroup);
            if (query.IsNullOrEmpty())
            {
                var pa = new PlusOneAvailable
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNumber = fromGroup,
                    Available = state
                };
                MongoService<PlusOneAvailable>.Insert(pa);
            }
            else
            {
                var ra = query.FirstOrDefault();
                Debug.Assert(ra != null, nameof(ra) + " != null");
                ra.Available = state;

                MongoService<PlusOneAvailable>.Update(ra);
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
            lock (List_lock)
            {
                return !this.InactiveGroups.Contains(GroupNum);
            }
        }
    }
}
