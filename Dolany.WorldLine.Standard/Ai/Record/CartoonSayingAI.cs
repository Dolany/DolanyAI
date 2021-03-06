﻿using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.Ai.Record
{
    public class CartoonSayingAI : AIBase
    {
        public override string AIName { get; set; } = "语录";

        public override string Description { get; set; } = "AI for Cartoon Sayings.";

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.语录功能;

        [EnterCommand(ID = "CartoonSayingAI_ProcceedMsg",
            Command = "语录",
            Description = "录入一条语录",
            SyntaxHint = "[出处] [人物] [内容]",
            SyntaxChecker = "Word Word Any",
            DailyLimit = 5)]
        public bool ProcceedMsg(MsgInformationEx MsgDTO, object[] param)
        {
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

        [EnterCommand(ID = "CartoonSayingAI_Sayings",
            Command = "语录",
            Description = "返回一条随机语录",
            DailyLimit = 10,
            TestingDailyLimit = 20)]
        public bool Sayings(MsgInformationEx MsgDTO, object[] param)
        {
            SayingRequest(MsgDTO);
            return true;
        }

        [EnterCommand(ID = "CartoonSayingAI_Sayings_Query",
            Command = "语录",
            Description = "按关键字检索语录",
            SyntaxHint = "[关键字]",
            SyntaxChecker = "Word",
            DailyLimit = 10,
            TestingDailyLimit = 20)]
        public bool Sayings_Query(MsgInformationEx MsgDTO, object[] param)
        {
            SayingRequest(MsgDTO, param[0] as string);
            return true;
        }

        private static void SayingRequest(MsgInformationEx MsgDTO, string keyword = null)
        {
            var ranSaying = GetRanSaying(MsgDTO.FromGroup, keyword);
            if (string.IsNullOrEmpty(ranSaying))
            {
                MsgSender.PushMsg(MsgDTO, "没有任何相关语录！");
                return;
            }

            MsgSender.PushMsg(MsgDTO, ranSaying);
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

        [EnterCommand(ID = "CartoonSayingAI_ClearSayings",
            Command = "删除语录",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "按关键字删除语录",
            SyntaxHint = "[关键字]",
            SyntaxChecker = "Word")]
        public bool ClearSayings(MsgInformationEx MsgDTO, object[] param)
        {
            var query = MongoService<Saying>.Get(s => s.FromGroup == MsgDTO.FromGroup &&
                                                      (s.Content.Contains(MsgDTO.Msg) ||
                                                       s.Charactor.Contains(MsgDTO.Msg) ||
                                                       s.Cartoon.Contains(MsgDTO.Msg)));
            var count = query.Count;
            MongoService<Saying>.DeleteMany(query);

            MsgSender.PushMsg(MsgDTO, $"共删除{count}条语录");
            return true;
        }
    }
}
