﻿using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai.Repeater
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Base;

    using Cache;
    using Dolany.Ai.Common;
    using Database;
    using Dolany.Database.Ai;

    using Model;

    using SyntaxChecker;

    [AI(
        Name = "随机复读",
        Description = "AI for Repeating Random words.",
        Enable = true,
        PriorityLevel = 1)]
    public class RepeatorAI : AIBase
    {
        private const long RepeatLimit = 30;

        private long CurCount;

        private const int SleepTime = 2000;

        private readonly List<long> InactiveGroups = new List<long>() {0};

        private readonly object List_lock = new object();

        public override void Initialization()
        {
            Load();
        }

        private void Load()
        {
            var query = MongoService<RepeaterAvailable>.Get(p => !p.Available);
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

            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            if (!IsAvailable(MsgDTO.FromGroup))
            {
                return false;
            }

            var atChecker = new AtChecker();
            if (atChecker.Check(MsgDTO.FullMsg, out _))
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
            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = Attr.Name,
                Command = "RepeatorOverride",
                GroupNum = MsgDTO.FromGroup
            });
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
        public bool Forbidden(MsgInformationEx MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, false);

            MsgSender.Instance.PushMsg(MsgDTO, "复读机禁用成功！");
            return true;
        }

        [EnterCommand(
            Command = "复读机启用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "重新启用复读机功能",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool Unforbidden(MsgInformationEx MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.FromGroup, true);

            MsgSender.Instance.PushMsg(MsgDTO, "复读机启用成功！");
            return true;
        }

        private void ForbiddenStateChange(long fromGroup, bool state)
        {
            var query = MongoService<RepeaterAvailable>.Get(r => r.GroupNumber == fromGroup);
            if (query.IsNullOrEmpty())
            {
                MongoService<RepeaterAvailable>.Insert(new RepeaterAvailable
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
