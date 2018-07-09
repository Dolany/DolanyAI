using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AILib.Entities;
using Flexlive.CQP.Framework.Utils;
using System.ComponentModel.Composition;

namespace AILib
{
    [Export(typeof(AIBase))]
    [AI(
        Name = "CartoonSayingAI",
        Description = "AI for Cartoon Sayings.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
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

        public CartoonSayingAI()
            : base()
        {
            RuntimeLogger.Log("CartoonSayingAI constructed");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "语录",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "录入语录或者按关键字检索语录",
            Syntax = " 或者 语录 [关键字]; 语录 [出处] [人物] [内容]",
            Tag = "语录功能",
            SyntaxChecker = "ProcceedMsg"
            )]
        public void ProcceedMsg(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("AlermClockAI Tryto ProcceedMsg In CartoonSayings");
            if (IsInSealing(MsgDTO.fromGroup, MsgDTO.fromQQ))
            {
                return;
            }

            switch ((int)param[0])
            {
                case 1:
                    string smsg = SaveSaying(param[1] as SayingEntity, MsgDTO.fromGroup) ? "语录录入成功！" : "语录录入失败！";
                    MsgSender.Instance.PushMsg(new SendMsgDTO()
                    {
                        Aim = MsgDTO.fromGroup,
                        Type = MsgType.Group,
                        Msg = smsg
                    });
                    break;

                case 2:
                    SayingRequest(MsgDTO);
                    break;

                case 3:
                    SayingRequest(MsgDTO, param[1] as string);
                    break;
            }

            RuntimeLogger.Log("AlermClockAI ProcceedMsg Completed In CartoonSayings");
        }

        private void SayingRequest(GroupMsgDTO MsgDTO, string keyword = null)
        {
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

        private bool IsInSealing(long groupNum, long memberNum)
        {
            var query = DbMgr.Query<SayingSealEntity>(s => s.GroupNum == groupNum && s.SealMember == memberNum);
            return !query.IsNullOrEmpty();
        }

        private bool SaveSaying(SayingEntity info, long fromGroup)
        {
            info.FromGroup = fromGroup;
            info.Id = Guid.NewGuid().ToString();

            DbMgr.Insert(info);
            return true;
        }

        private string GetRanSaying(long fromGroup, string keyword = null)
        {
            var list = SayingList;
            var query = from saying in list
                        where (string.IsNullOrEmpty(keyword) ? true : saying.Contains(keyword))
                            && (fromGroup == 0 ? true : saying.FromGroup == fromGroup)
                        select saying;
            if (query.IsNullOrEmpty())
            {
                return string.Empty;
            }
            list = query.ToList();

            Random random = new Random();
            int randIdx = random.Next(list.Count);

            return GetShownSaying(list[randIdx]);
        }

        private string GetShownSaying(SayingEntity s)
        {
            string shownSaying = $@"
    {s.Content}
    ——《{s.Cartoon}》 {s.Charactor}
";

            return shownSaying;
        }

        [EnterCommand(
            Command = "语录总数",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "查询录入的所有语录的总数",
            Syntax = "",
            Tag = "语录功能",
            SyntaxChecker = "Empty"
            )]
        public void SayingTotalCount(PrivateMsgDTO MsgDTO, object[] param)
        {
            Common.SendMsgToDeveloper($@"共有语录 {SayingList.Count}条");
        }

        [EnterCommand(
            Command = "删除语录",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.群主,
            Description = "按关键字删除语录",
            Syntax = "[关键字]",
            Tag = "语录功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void ClearSayings(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("AlermClockAI Tryto ClearSayings");
            int delCount = DbMgr.Delete<SayingEntity>(s => s.FromGroup == MsgDTO.fromGroup
                                                        && (s.Content.Contains(MsgDTO.msg)
                                                        || s.Charactor.Contains(MsgDTO.msg)
                                                        || s.Cartoon.Contains(MsgDTO.msg)));

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = $"共删除{delCount}条语录"
            });
            RuntimeLogger.Log("AlermClockAI ClearSayings Complete");
        }

        [EnterCommand(
            Command = "语录封禁",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.群主,
            Description = "封禁一个群员，让他无法使用语录功能",
            Syntax = "[qq号码]",
            Tag = "语录功能",
            SyntaxChecker = "Long"
            )]
        public void SayingSeal(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("AlermClockAI Tryto SayingSeal");
            long memberNum;
            if (!long.TryParse(MsgDTO.msg, out memberNum))
            {
                return;
            }

            var query = DbMgr.Query<SayingSealEntity>(s => s.GroupNum == MsgDTO.fromGroup && s.SealMember == memberNum);
            if (!query.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "此成员正在封禁中！"
                });

                return;
            }

            DbMgr.Insert(new SayingSealEntity()
            {
                Id = Guid.NewGuid().ToString(),
                CreateTime = DateTime.Now,
                SealMember = memberNum,
                GroupNum = MsgDTO.fromGroup,
                Content = "封禁"
            });
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "封禁成功！"
            });
            RuntimeLogger.Log("AlermClockAI SayingSeal Complete");
        }

        [EnterCommand(
            Command = "语录解封",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.群主,
            Description = "解封一个群员，让他可以继续使用语录功能",
            Syntax = "[qq号码]",
            Tag = "语录功能",
            SyntaxChecker = "Long"
            )]
        public void SayingDeseal(GroupMsgDTO MsgDTO, object[] param)
        {
            RuntimeLogger.Log("AlermClockAI Tryto SayingDeseal");
            long memberNum;
            if (!long.TryParse(MsgDTO.msg, out memberNum))
            {
                return;
            }

            int delCount = DbMgr.Delete<SayingSealEntity>(s => s.GroupNum == MsgDTO.fromGroup && s.SealMember == memberNum);
            if (delCount == 0)
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "此成员尚未被封禁！"
                });

                return;
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "解封成功！"
            });
            RuntimeLogger.Log("AlermClockAI SayingDeseal Complete");
        }
    }
}