using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AILib.Entities;
using Flexlive.CQP.Framework.Utils;
using System.ComponentModel.Composition;

namespace AILib
{
    [Export(typeof(AIBase))]
    [AI(
        Name = "PlusOneAI",
        Description = "AI for Auto Plus One.",
        IsAvailable = true,
        PriorityLevel = 0
        )]
    public class PlusOneAI : AIBase
    {
        private List<PlusOneCache> Cache = new List<PlusOneCache>();

        public PlusOneAI()
            : base()
        {
            RuntimeLogger.Log("PlusOneAI started");
        }

        public override void Work()
        {
        }

        public override bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if (!IsAvailable(MsgDTO.fromGroup))
            {
                return false;
            }
            if (MsgDTO.fullMsg.Contains("CQ:at"))
            {
                return false;
            }

            var query = Cache.Where(d => d.GroupNumber == MsgDTO.fromGroup);
            if (query.IsNullOrEmpty())
            {
                Cache.Add(new PlusOneCache
                {
                    GroupNumber = MsgDTO.fromGroup,
                    IsAlreadyRepeated = false,
                    MsgCache = MsgDTO.fullMsg
                });

                return false;
            }

            Thread.Sleep(2000);
            var groupCache = query.FirstOrDefault();
            Repeat(MsgDTO.fromGroup, MsgDTO.fullMsg, groupCache);
            return true;
        }

        private void Repeat(long fromGroup, string FullMsg, PlusOneCache groupCache)
        {
            if (groupCache.MsgCache != FullMsg)
            {
                groupCache.MsgCache = FullMsg;
                groupCache.IsAlreadyRepeated = false;

                return;
            }
            if (groupCache.IsAlreadyRepeated)
            {
                return;
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = fromGroup,
                Type = MsgType.Group,
                Msg = FullMsg
            });
            groupCache.IsAlreadyRepeated = true;
        }

        [EnterCommand(
            Command = "+1复读禁用",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.群主,
            Description = "禁用+1复读功能，禁用后将不会在本群进行+1复读",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty"
            )]
        public void Forbidden(GroupMsgDTO MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.fromGroup, false);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "+1复读禁用成功！"
            });
        }

        [EnterCommand(
            Command = "+1复读启用",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.群主,
            Description = "重新启用+1复读功能",
            Syntax = "",
            Tag = "复读机功能",
            SyntaxChecker = "Empty"
            )]
        public void Unforbidden(GroupMsgDTO MsgDTO, object[] param)
        {
            ForbiddenStateChange(MsgDTO.fromGroup, true);

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "+1复读启用成功！"
            });
        }

        private void ForbiddenStateChange(long fromGroup, bool state)
        {
            var query = DbMgr.Query<PlusOneAvailableEntity>(r => r.GroupNumber == fromGroup);
            if (query.IsNullOrEmpty())
            {
                DbMgr.Insert(new PlusOneAvailableEntity()
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
            var query = DbMgr.Query<PlusOneAvailableEntity>();
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
    }
}