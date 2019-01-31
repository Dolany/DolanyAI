﻿using Dolany.Ai.Common;

namespace Dolany.Ai.Reborn.DolanyAI.Ai.Game.TouhouCard
{
    using System.IO;
    using System.Linq;

    using Core.Base;
    using Core.Cache;
    using Core.Model;
    using Database.Sqlite;
    using Database.Sqlite.Model;

    using static Core.API.CodeApi;

    [AI(
        Name = nameof(TouhouCardAi),
        Description = "AI for Getting Random TouhouCard.",
        Enable = true,
        PriorityLevel = 10)]
    public class TouhouCardAi : AIBase
    {
        private const string PicPath = "TouhouCard/";

        public override void Initialization()
        {
        }

        [EnterCommand(
            Command = ".card 幻想乡抽卡",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机获取一张DIY幻想乡卡牌",
            Syntax = "",
            Tag = "游戏功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public void RandomCard(MsgInformationEx MsgDTO, object[] param)
        {
            var cardName = RandomCard(MsgDTO.FromQQ);
            MsgSender.Instance.PushMsg(MsgDTO, Code_Image_Relational(cardName));
        }

        private static string RandomCard(long FromQQ)
        {
            var key = $"TouhouCard-{FromQQ}";
            var cache = SCacheService.Get<TouhouCardCache>(key);
            if (cache != null)
            {
                return PicPath + cache.CardName;
            }

            var tcr = new TouhouCardCache { QQNum = FromQQ, CardName = GetRandCard() };
            SCacheService.Cache(key, tcr);

            return PicPath + tcr.CardName;
        }

        private static string GetRandCard()
        {
            var dir = new DirectoryInfo(PicPath);
            var files = dir.GetFiles();
            var rIdx = CommonUtil.RandInt(files.Length);
            return files.ElementAt(rIdx).Name;
        }
    }
}
