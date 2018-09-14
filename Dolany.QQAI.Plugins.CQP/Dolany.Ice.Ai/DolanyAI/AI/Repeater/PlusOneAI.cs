﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Threading;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(PlusOneAI),
        Description = "AI for Auto Plus One.",
        IsAvailable = true,
        PriorityLevel = 0
        )]
    public class PlusOneAI : AIBase
    {
        private List<PlusOneCache> Cache { get; } = new List<PlusOneCache>();

        public PlusOneAI()
        {
            RuntimeLogger.Log("PlusOneAI started");
        }

        public override void Work()
        {
        }

        public override bool OnMsgReceived(ReceivedMsgDTO MsgDTO)
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

            var query = Cache.Where(d => d.GroupNumber == MsgDTO.FromGroup);
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

        private static void Repeat(ReceivedMsgDTO MsgDTO, PlusOneCache groupCache)
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
            IsDeveloperOnly = false,
            IsPrivateAvailabe = false
            )]
        public void Forbidden(ReceivedMsgDTO MsgDTO, object[] param)
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
            IsDeveloperOnly = false,
            IsPrivateAvailabe = false
            )]
        public void Unforbidden(ReceivedMsgDTO MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, true);

            MsgSender.Instance.PushMsg(MsgDTO, "+1复读启用成功！");
        }

        private static void ForbiddenStateChange(long fromGroup, bool state)
        {
            using (var db = new AIDatabase())
            {
                var query = db.PlusOneAvailable.Where(r => r.GroupNumber == fromGroup);
                if (query.IsNullOrEmpty())
                {
                    var pa = new PlusOneAvailable
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupNumber = fromGroup,
                        Available = state
                    };
                    db.PlusOneAvailable.Add(pa);
                }
                else
                {
                    var ra = query.FirstOrDefault();
                    Debug.Assert(ra != null, nameof(ra) + " != null");
                    ra.Available = state;
                }

                db.SaveChanges();
            }
        }

        private static bool IsAvailable(long GroupNum)
        {
            using (var db = new AIDatabase())
            {
                var query = db.PlusOneAvailable.Where(r => r.GroupNumber == GroupNum &&
                                                           !r.Available);
                return query.IsNullOrEmpty();
            }
        }
    }
}