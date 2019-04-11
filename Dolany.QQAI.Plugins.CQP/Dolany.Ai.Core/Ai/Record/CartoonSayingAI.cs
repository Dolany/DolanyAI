namespace Dolany.Ai.Core.Ai.Record
{
    using System;
    using System.Linq;

    using Base;

    using Cache;
    using Dolany.Ai.Common;
    using Database;
    using Dolany.Database.Ai;

    using Model;

    [AI(
        Name = "语录",
        Description = "AI for Cartoon Sayings.",
        Enable = true,
        PriorityLevel = 10)]
    public class CartoonSayingAI : AIBase
    {
        public override void Initialization()
        {
        }

        [EnterCommand(
            Command = "语录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "录入一条语录",
            Syntax = "[出处] [人物] [内容]",
            Tag = "语录功能",
            SyntaxChecker = "Word Word Any",
            IsPrivateAvailable = false,
            DailyLimit = 5)]
        public bool ProcceedMsg(MsgInformationEx MsgDTO, object[] param)
        {
            if (IsInSealing(MsgDTO.FromGroup, MsgDTO.FromQQ))
            {
                return false;
            }

            var saying = new Saying
            {
                Id = Guid.NewGuid().ToString(),
                Cartoon = param[0] as string,
                Charactor = param[1] as string,
                Content = param[2] as string,
                FromGroup = MsgDTO.FromGroup
            };

            MongoService<Saying>.Insert(saying);

            MsgSender.PushMsg(MsgDTO, "语录录入成功！");
            return true;
        }

        [EnterCommand(
            Command = "语录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "返回一条随机语录",
            Syntax = "",
            Tag = "语录功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false,
            DailyLimit = 10,
            TestingDailyLimit = 20)]
        public bool Sayings(MsgInformationEx MsgDTO, object[] param)
        {
            if (IsInSealing(MsgDTO.FromGroup, MsgDTO.FromQQ))
            {
                return false;
            }

            SayingRequest(MsgDTO);
            return true;
        }

        [EnterCommand(
            Command = "语录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "按关键字检索语录",
            Syntax = "[关键字]",
            Tag = "语录功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false,
            DailyLimit = 10,
            TestingDailyLimit = 20)]
        public bool Sayings_Query(MsgInformationEx MsgDTO, object[] param)
        {
            if (IsInSealing(MsgDTO.FromGroup, MsgDTO.FromQQ))
            {
                return false;
            }

            SayingRequest(MsgDTO, param[0] as string);
            return true;
        }

        private static void SayingRequest(MsgInformationEx MsgDTO, string keyword = null)
        {
            var ranSaying = GetRanSaying(MsgDTO.FromGroup, keyword);
            if (string.IsNullOrEmpty(ranSaying))
            {
                return;
            }

            MsgSender.PushMsg(MsgDTO, ranSaying);
        }

        private static bool IsInSealing(long groupNum, long memberNum)
        {
            var query = MongoService<SayingSeal>.Get(s => s.GroupNum == groupNum &&
                                                          s.SealMember == memberNum);
            return !query.IsNullOrEmpty();
        }

        private static string GetRanSaying(long fromGroup, string keyword = null)
        {
            var query = MongoService<Saying>.Get(p => p.FromGroup == fromGroup);
            if (keyword != null)
            {
                query = query.Where(p => p.Cartoon.Contains(keyword) ||
                                         p.Charactor.Contains(keyword) ||
                                         p.Content.Contains(keyword)).ToList();
            }

            if (query.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var saying = query.RandElement();
            return GetShownSaying(saying);
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
            IsPrivateAvailable = false)]
        public bool ClearSayings(MsgInformationEx MsgDTO, object[] param)
        {
            var query = MongoService<Saying>.Get(s => s.FromGroup == MsgDTO.FromGroup &&
                                                      (s.Content.Contains(MsgDTO.Msg) ||
                                                       s.Charactor.Contains(MsgDTO.Msg) ||
                                                       s.Cartoon.Contains(MsgDTO.Msg)));
            var count = query.Count();
            MongoService<Saying>.DeleteMany(query);

            MsgSender.PushMsg(MsgDTO, $"共删除{count}条语录");
            return true;
        }

        [EnterCommand(
            Command = "语录封禁",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "封禁一个群员，让他无法使用语录功能",
            Syntax = "[@qq号码]",
            Tag = "语录功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false)]
        public bool SayingSeal(MsgInformationEx MsgDTO, object[] param)
        {
            var memberNum = (long)param[0];

            var query = MongoService<SayingSeal>.Get(s => s.GroupNum == MsgDTO.FromGroup &&
                                                          s.SealMember == memberNum);
            if (!query.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "此成员正在封禁中！");
                return false;
            }

            MongoService<SayingSeal>.Insert(new SayingSeal
            {
                Id = Guid.NewGuid().ToString(),
                CreateTime = DateTime.Now,
                SealMember = memberNum,
                GroupNum = MsgDTO.FromGroup,
                Content = "封禁"
            });
            MsgSender.PushMsg(MsgDTO, "封禁成功！");
            return true;
        }

        [EnterCommand(
            Command = "语录解封",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "解封一个群员，让他可以继续使用语录功能",
            Syntax = "[@qq号码]",
            Tag = "语录功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false)]
        public bool SayingDeseal(MsgInformationEx MsgDTO, object[] param)
        {
            var memberNum = (long)param[0];

            var query = MongoService<SayingSeal>.Get(s => s.GroupNum == MsgDTO.FromGroup &&
                                                          s.SealMember == memberNum);
            if (query.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "此成员尚未被封禁！");
                return false;
            }
            MongoService<SayingSeal>.DeleteMany(query);

            MsgSender.PushMsg(MsgDTO, "解封成功！");
            return true;
        }
    }
}
