using System;
using System.IO;
using System.Linq;

namespace Dolany.Ai.Reborn.DolanyAI.Ai.Game.TouhouCard
{
    using Core.Base;
    using Core.Cache;
    using Core.Common;
    using Core.Model;
    using Dolany.Database.Ai;

    using static Core.API.CodeApi;

    [AI(
        Name = nameof(TouhouCardAi),
        Description = "AI for Getting Random TouhouCard.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class TouhouCardAi : AIBase
    {
        private const string PicPath = "TouhouCard/";

        public TouhouCardAi()
        {
            RuntimeLogger.Log("TouhouCardAI started");
        }

        public override void Work()
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
            MsgSender.Instance.PushMsg(MsgDTO, Code_Image(new FileInfo(cardName).FullName));
        }

        private static string RandomCard(long FromQQ)
        {
            using (var db = new AIDatabase())
            {
                var query = db.TouhouCardRecord.Where(p => p.QQNum == FromQQ);
                if (query.IsNullOrEmpty())
                {
                    var tcr = new TouhouCardRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        UpdateDate = DateTime.Now.Date,
                        CardName = GetRandCard(),
                        QQNum = FromQQ
                    };
                    db.TouhouCardRecord.Add(tcr);
                    db.SaveChanges();

                    return PicPath + tcr.CardName;
                }

                var rec = query.First();
                if (rec.UpdateDate >= DateTime.Now.Date)
                {
                    return PicPath + rec.CardName;
                }

                rec.CardName = GetRandCard();
                rec.UpdateDate = DateTime.Now.Date;
                db.SaveChanges();

                return PicPath + rec.CardName;
            }
        }

        private static string GetRandCard()
        {
            var dir = new DirectoryInfo(PicPath);
            var files = dir.GetFiles();
            var rIdx = Utility.RandInt(files.Length);
            return files.ElementAt(rIdx).Name;
        }
    }
}
