using System;
using System.IO;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Linq;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(TouhouCardAI),
        Description = "AI for Getting Random TouhouCard.",
        IsAvailable = true,
        PriorityLevel = 10
    )]
    public class TouhouCardAI : AIBase
    {
        private const string PicPath = "./TouhouCard/";

        public TouhouCardAI()
        {
            RuntimeLogger.Log("TouhouCardAI started");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "幻想乡抽卡",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机获取一张DIY幻想乡卡牌",
            Syntax = "",
            Tag = "游戏功能",
            SyntaxChecker = "Empty"
        )]
        public void RandomCard(GroupMsgDTO MsgDTO, object[] param)
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

        private static void ReturnCard(GroupMsgDTO MsgDTO, string cardName)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = CodeApi.Code_Image(PicPath + cardName)
            });
        }

        private static string GetRandCard()
        {
            var dir = new DirectoryInfo(PicPath);
            var files = dir.GetFiles();
            var rand = new Random();
            var rIdx = rand.Next(files.Length);
            return files.ElementAt(rIdx).Name;
        }
    }
}