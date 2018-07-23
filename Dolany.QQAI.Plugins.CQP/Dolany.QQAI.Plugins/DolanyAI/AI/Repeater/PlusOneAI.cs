using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.QQAI.Plugins.DolanyAI.Db;
using System.Threading;

namespace Dolany.QQAI.Plugins.DolanyAI
{
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
            if (base.OnGroupMsgReceived(MsgDTO))
            {
                return true;
            }

            if (!IsAvailable(MsgDTO.FromGroup))
            {
                return false;
            }
            if (MsgDTO.FullMsg.Contains("CQ:at"))
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
            Repeat(MsgDTO.FromGroup, MsgDTO.FullMsg, groupCache);
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

        [GroupEnterCommand(
            Command = "+1复读禁用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "禁用+1复读功能，禁用后将不会在本群进行+1复读",
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
                Msg = "+1复读禁用成功！"
            });
        }

        [GroupEnterCommand(
            Command = "+1复读启用",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "重新启用+1复读功能",
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
                Msg = "+1复读启用成功！"
            });
        }

        private void ForbiddenStateChange(long fromGroup, bool state)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.PlusOneAvailable.Where(r => r.GroupNumber == fromGroup);
                if (query.IsNullOrEmpty())
                {
                    PlusOneAvailable pa = new PlusOneAvailable
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
                    ra.Available = state;
                }

                db.SaveChanges();
            }
        }

        private bool IsAvailable(long GroupNum)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.PlusOneAvailable.Where(r => r.GroupNumber == GroupNum && !r.Available);
                return query.IsNullOrEmpty();
            }
        }
    }
}