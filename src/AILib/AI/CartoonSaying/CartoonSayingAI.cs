using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AILib.Entities;

namespace AILib
{
    [AI(Name = "CartoonSayingAI", Description = "AI for Cartoon Sayings.", IsAvailable = true)]
    public class CartoonSayingAI : AIBase
    {
        private List<SayingEntity> SayingList
        {
            get
            {
                var query = DbMgr.Query<SayingEntity>();
                return query == null ? null : query.ToList();
            }
        }

        public CartoonSayingAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
        }

        public override void Work()
        {
        }

        [EnterCommand(Command = "语录", SourceType = MsgType.Group)]
        public void ProcceedMsg(GroupMsgDTO MsgDTO)
        {
            SayingEntity info = SayingEntity.Parse(MsgDTO.msg);
            if (info != null)
            {
                string smsg = SaveSaying(info, MsgDTO.fromGroup) ? "语录录入成功！" : "语录录入失败！";
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = smsg
                });
                return;
            }

            string keyword = string.IsNullOrEmpty(MsgDTO.msg.Trim()) ? null : MsgDTO.msg;
            string ranSaying = GetRanSaying(MsgDTO.fromGroup, keyword);
            if (string.IsNullOrEmpty(ranSaying))
            {
                return;
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = ranSaying
            });
        }

        private bool SaveSaying(SayingEntity info, long fromGroup)
        {
            info.FromGroup = fromGroup;
            info.Id = Guid.NewGuid().ToString();

            try
            {
                DbMgr.Insert(info);
                return true;
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
                return false;
            }
        }

        private string GetRanSaying(long fromGroup, string keyword = null)
        {
            try
            {
                var list = SayingList;
                var query = from saying in list
                            where (string.IsNullOrEmpty(keyword) ? true : saying.Contains(keyword))
                                && (fromGroup == 0 ? true : saying.FromGroup == fromGroup)
                            select saying;
                if (query == null || query.Count() == 0)
                {
                    return string.Empty;
                }
                list = query.ToList();

                Random random = new Random();
                int randIdx = random.Next(list.Count);

                return GetShownSaying(list[randIdx]);
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
                return string.Empty;
            }
        }

        private string GetShownSaying(SayingEntity s)
        {
            string shownSaying = $@"
    {s.Content}
    ——《{s.Cartoon}》 {s.Charactor}
";

            return shownSaying;
        }

        [EnterCommand(Command = "语录", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void SayingTotalCount(PrivateMsgDTO MsgDTO)
        {
            if (MsgDTO.msg == "总数")
            {
                Common.SendMsgToDeveloper($@"共有语录 {SayingList.Count}条");
            }
        }

        [EnterCommand(Command = "清空语录", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.群主)]
        public void ClearSayings(GroupMsgDTO MsgDTO)
        {
        }

        [EnterCommand(Command = "语录封禁", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.群主)]
        public void SayingSeal(GroupMsgDTO MsgDTO)
        {
        }

        [EnterCommand(Command = "语录解封", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.群主)]
        public void SayingDeseal(GroupMsgDTO MsgDTO)
        {
        }
    }
}