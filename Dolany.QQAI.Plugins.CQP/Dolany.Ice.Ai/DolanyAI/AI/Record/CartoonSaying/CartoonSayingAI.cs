using System;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.DolanyAI.Utils;

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
        public CartoonSayingAI()
        {
            RuntimeLogger.Log("CartoonSayingAI constructed");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "语录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "录入一条语录",
            Syntax = "[出处] [人物] [内容]",
            Tag = "语录功能",
            SyntaxChecker = "Word Word Word",
            IsPrivateAvailabe = false
            )]
        public void ProcceedMsg(ReceivedMsgDTO MsgDTO, object[] param)
        {
            if (IsInSealing(MsgDTO.FromGroup, MsgDTO.FromQQ))
            {
                return;
            }

            var saying = new Saying
            {
                Id = Guid.NewGuid().ToString(),
                Cartoon = param[0] as string,
                Charactor = param[1] as string,
                Content = param[2] as string,
                FromGroup = MsgDTO.FromGroup
            };
            using (var db = new AIDatabase())
            {
                db.Saying.Add(saying);
                db.SaveChanges();
            }

            MsgSender.Instance.PushMsg(MsgDTO, "语录录入成功！");
        }

        [EnterCommand(
            Command = "语录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "返回一条随机语录",
            Syntax = "",
            Tag = "语录功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false
        )]
        public void Sayings(ReceivedMsgDTO MsgDTO, object[] param)
        {
            if (IsInSealing(MsgDTO.FromGroup, MsgDTO.FromQQ))
            {
                return;
            }

            SayingRequest(MsgDTO);
        }

        [EnterCommand(
            Command = "语录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "按关键字检索语录",
            Syntax = "[关键字]",
            Tag = "语录功能",
            SyntaxChecker = "Word",
            IsPrivateAvailabe = false
        )]
        public void Sayings_Query(ReceivedMsgDTO MsgDTO, object[] param)
        {
            if (IsInSealing(MsgDTO.FromGroup, MsgDTO.FromQQ))
            {
                return;
            }

            SayingRequest(MsgDTO, param[0] as string);
        }

        private static void SayingRequest(ReceivedMsgDTO MsgDTO, string keyword = null)
        {
            var ranSaying = GetRanSaying(MsgDTO.FromGroup, keyword);
            if (string.IsNullOrEmpty(ranSaying))
            {
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, ranSaying);
        }

        private static bool IsInSealing(long groupNum, long memberNum)
        {
            using (var db = new AIDatabase())
            {
                var query = db.SayingSeal.Where(s => s.GroupNum == groupNum &&
                                                     s.SealMember == memberNum);
                return !query.IsNullOrEmpty();
            }
        }

        private static string GetRanSaying(long fromGroup, string keyword = null)
        {
            using (var db = new AIDatabase())
            {
                var query = db.Saying.Where(p => p.FromGroup == fromGroup);
                if (keyword != null)
                {
                    query = query.Where(p => p.Cartoon.Contains(keyword) ||
                                             p.Charactor.Contains(keyword) ||
                                             p.Content.Contains(keyword));
                }

                if (query.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                query = query.OrderBy(p => p.Id);
                var randIdx = Utility.RandInt(query.Count());
                var saying = query.Skip(randIdx).FirstOrDefault();
                return GetShownSaying(saying);
            }
        }

        private static string GetShownSaying(Saying s)
        {
            var shownSaying = $@"
    {s.Content}
    ——《{s.Cartoon}》 {s.Charactor}
";

            return shownSaying;
        }

        [EnterCommand(
            Command = "删除语录",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "按关键字删除语录",
            Syntax = "[关键字]",
            Tag = "语录功能",
            SyntaxChecker = "Word",
            IsPrivateAvailabe = false
            )]
        public void ClearSayings(ReceivedMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.Saying.Where(s => s.FromGroup == MsgDTO.FromGroup &&
                                                 (s.Content.Contains(MsgDTO.Msg) ||
                                                  s.Charactor.Contains(MsgDTO.Msg) ||
                                                  s.Cartoon.Contains(MsgDTO.Msg)));
                var count = query.Count();
                db.Saying.RemoveRange(query);
                db.SaveChanges();

                MsgSender.Instance.PushMsg(MsgDTO, $"共删除{count}条语录");
            }
        }

        [EnterCommand(
            Command = "语录封禁",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "封禁一个群员，让他无法使用语录功能",
            Syntax = "[@qq号码]",
            Tag = "语录功能",
            SyntaxChecker = "At",
            IsPrivateAvailabe = false
            )]
        public void SayingSeal(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var memberNum = (long)param[0];

            using (var db = new AIDatabase())
            {
                var query = db.SayingSeal.Where(s => s.GroupNum == MsgDTO.FromGroup &&
                                                     s.SealMember == memberNum);
                if (!query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(MsgDTO, "此成员正在封禁中！");
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
            MsgSender.Instance.PushMsg(MsgDTO, "封禁成功！");
        }

        [EnterCommand(
            Command = "语录解封",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "解封一个群员，让他可以继续使用语录功能",
            Syntax = "[@qq号码]",
            Tag = "语录功能",
            SyntaxChecker = "At",
            IsPrivateAvailabe = false
            )]
        public void SayingDeseal(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var memberNum = (long)param[0];

            using (var db = new AIDatabase())
            {
                var query = db.SayingSeal.Where(s => s.GroupNum == MsgDTO.FromGroup &&
                                                     s.SealMember == memberNum);
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(MsgDTO, "此成员尚未被封禁！");
                    return;
                }
                foreach (var s in query)
                {
                    db.SayingSeal.Remove(s);
                }
                db.SaveChanges();
            }

            MsgSender.Instance.PushMsg(MsgDTO, "解封成功！");
        }
    }
}