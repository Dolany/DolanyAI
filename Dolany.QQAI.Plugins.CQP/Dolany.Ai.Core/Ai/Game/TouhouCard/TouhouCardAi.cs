﻿using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;

namespace DolanyAI.Ai.Game.TouhouCard
{
    using System.IO;

    [AI(
        Name = "幻想乡抽卡",
        Description = "AI for Getting Random TouhouCard.",
        Enable = true,
        PriorityLevel = 10)]
    public class TouhouCardAi : AIBase
    {
        private const string PicPath = "TouhouCard/";

        public override void Initialization()
        {
        }

        [EnterCommand(ID = "TouhouCardAi_RandomCard",
            Command = ".card 幻想乡抽卡",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机获取一张DIY幻想乡卡牌(每日刷新)",
            Syntax = "",
            Tag = "游戏功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool RandomCard(MsgInformationEx MsgDTO, object[] param)
        {
            var cardName = RandomCard(MsgDTO.FromQQ);
            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Image_Relational(cardName));
            return true;
        }

        private static string RandomCard(long FromQQ)
        {
            var cache = PersonCacheRecord.Get(FromQQ, "TouhouCard");
            if (!string.IsNullOrEmpty(cache.Value))
            {
                return PicPath + cache.Value;
            }

            var card = GetRandCard();
            cache.Value = card;
            cache.ExpiryTime = CommonUtil.UntilTommorow();
            cache.Update();

            return PicPath + card;
        }

        private static string GetRandCard()
        {
            var dir = new DirectoryInfo(PicPath);
            var files = dir.GetFiles();
            var file = files.RandElement();
            return file.Name;
        }
    }
}
