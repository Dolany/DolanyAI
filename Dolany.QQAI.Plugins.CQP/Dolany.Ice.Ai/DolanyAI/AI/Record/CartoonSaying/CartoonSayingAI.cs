﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(CartoonSayingAI),
        Description = "AI for Cartoon Sayings.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class CartoonSayingAI : AIBase
    {
        private List<Saying> SayingList
        {
            get
            {
                using (AIDatabase db = new AIDatabase())
                {
                    var query = db.Saying;
                    return query.IsNullOrEmpty() ? null : query.ToList();
                }
            }
        }

        public CartoonSayingAI()
        {
            RuntimeLogger.Log("CartoonSayingAI constructed");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "语录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "录入语录或者按关键字检索语录",
            Syntax = " 或者 语录 [关键字]; 语录 [出处] [人物] [内容]",
            Tag = "语录功能",
            SyntaxChecker = nameof(ProcceedMsg)
            )]
        public void ProcceedMsg(GroupMsgDTO MsgDTO, object[] param)
        {
            if (IsInSealing(MsgDTO.FromGroup, MsgDTO.FromQQ))
            {
                return;
            }

            switch ((int)param[0])
            {
                case 1:
                    var smsg = SaveSaying(param[1] as Saying, MsgDTO.FromGroup) ? "语录录入成功！" : "语录录入失败！";
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
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

                default:
                    throw new Exception("Unexpected Case");
            }
        }

        private void SayingRequest(GroupMsgDTO MsgDTO, string keyword = null)
        {
            var ranSaying = GetRanSaying(MsgDTO.FromGroup, keyword);
            if (string.IsNullOrEmpty(ranSaying))
            {
                return;
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = ranSaying
            });
        }

        private static bool IsInSealing(long groupNum, long memberNum)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.SayingSeal.Where(s => s.GroupNum == groupNum && s.SealMember == memberNum);
                return !query.IsNullOrEmpty();
            }
        }

        private static bool SaveSaying(Saying info, long fromGroup)
        {
            info.FromGroup = fromGroup;
            info.Id = Guid.NewGuid().ToString();

            using (AIDatabase db = new AIDatabase())
            {
                db.Saying.Add(info);
                db.SaveChanges();
            }
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

            var random = new Random();
            var randIdx = random.Next(list.Count);

            return GetShownSaying(list[randIdx]);
        }

        private static string GetShownSaying(Saying s)
        {
            var shownSaying = $@"
    {s.Content}
    ——《{s.Cartoon}》 {s.Charactor}
";

            return shownSaying;
        }

        [PrivateEnterCommand(
            Command = "语录总数",
            Description = "查询录入的所有语录的总数",
            Syntax = "",
            Tag = "语录功能",
            SyntaxChecker = "Empty"
            )]
        public void SayingTotalCount(PrivateMsgDTO MsgDTO, object[] param)
        {
            Utility.SendMsgToDeveloper($@"共有语录 {SayingList.Count}条");
        }

        [GroupEnterCommand(
            Command = "删除语录",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "按关键字删除语录",
            Syntax = "[关键字]",
            Tag = "语录功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void ClearSayings(GroupMsgDTO MsgDTO, object[] param)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.Saying.Where(s => s.FromGroup == MsgDTO.FromGroup
                                                        && (s.Content.Contains(MsgDTO.Msg)
                                                        || s.Charactor.Contains(MsgDTO.Msg)
                                                        || s.Cartoon.Contains(MsgDTO.Msg)));
                db.Saying.RemoveRange(query);
                db.SaveChanges();

                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = $"共删除{query.Count()}条语录"
                });
            }
        }

        [GroupEnterCommand(
            Command = "语录封禁",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "封禁一个群员，让他无法使用语录功能",
            Syntax = "[qq号码]",
            Tag = "语录功能",
            SyntaxChecker = "Long"
            )]
        public void SayingSeal(GroupMsgDTO MsgDTO, object[] param)
        {
            if (!long.TryParse(MsgDTO.Msg, out long memberNum))
            {
                return;
            }

            using (AIDatabase db = new AIDatabase())
            {
                var query = db.SayingSeal.Where(s => s.GroupNum == MsgDTO.FromGroup && s.SealMember == memberNum);
                if (!query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "此成员正在封禁中！"
                    });

                    return;
                }

                db.SayingSeal.Add(new SayingSeal
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    SealMember = memberNum,
                    GroupNum = MsgDTO.FromGroup,
                    Content = "封禁"
                });

                db.SaveChanges();
            }
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "封禁成功！"
            });
        }

        [GroupEnterCommand(
            Command = "语录解封",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "解封一个群员，让他可以继续使用语录功能",
            Syntax = "[qq号码]",
            Tag = "语录功能",
            SyntaxChecker = "Long"
            )]
        public void SayingDeseal(GroupMsgDTO MsgDTO, object[] param)
        {
            if (!long.TryParse(MsgDTO.Msg, out long memberNum))
            {
                return;
            }

            using (AIDatabase db = new AIDatabase())
            {
                var query = db.SayingSeal.Where(s => s.GroupNum == MsgDTO.FromGroup && s.SealMember == memberNum);
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "此成员尚未被封禁！"
                    });

                    return;
                }
                foreach (var s in query)
                {
                    db.SayingSeal.Remove(s);
                }
                db.SaveChanges();
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "解封成功！"
            });
        }
    }
}