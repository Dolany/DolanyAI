using System;
using System.IO;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Linq;
using static Dolany.Ice.Ai.MahuaApis.CodeApi;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(TouhouCardAi),
        Description = "AI for Getting Random TouhouCard.",
        IsAvailable = true,
        PriorityLevel = 10
    )]
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
            Command = ".card",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机获取一张DIY幻想乡卡牌",
            Syntax = "",
            Tag = "抽卡功能",
            SyntaxChecker = "Empty",
            IsDeveloperOnly = false,
            IsPrivateAvailabe = true
        )]
        [EnterCommand(
            Command = "幻想乡抽卡",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机获取一张DIY幻想乡卡牌",
            Syntax = "",
            Tag = "抽卡功能",
            SyntaxChecker = "Empty",
            IsDeveloperOnly = false,
            IsPrivateAvailabe = true
        )]
        public void RandomCard(ReceivedMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.TouhouCardRecord.Where(p => p.QQNum == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    var tcr = new TouhouCardRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        UpdateDate = DateTime.Now.Date,
                        CardName = GetRandCard(),
                        QQNum = MsgDTO.FromQQ
                    };
                    db.TouhouCardRecord.Add(tcr);
                    db.SaveChanges();
                    ReturnCard(MsgDTO, tcr.CardName);

                    return;
                }

                var rec = query.First();
                if (rec.UpdateDate < DateTime.Now.Date)
                {
                    rec.CardName = GetRandCard();
                    rec.UpdateDate = DateTime.Now.Date;
                    db.SaveChanges();
                    ReturnCard(MsgDTO, rec.CardName);

                    return;
                }

                ReturnCard(MsgDTO, rec.CardName);
            }
        }

        private static void ReturnCard(ReceivedMsgDTO MsgDTO, string cardName)
        {
            MsgSender.Instance.PushMsg(MsgDTO, Code_Image(new FileInfo(PicPath + cardName).FullName));
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