using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(FortuneAI),
        Description = "AI for Fortune.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class FortuneAI : AIBase
    {
        private readonly string TarotServerPath = "https://m.sheup.com/";

        public FortuneAI()
        {
            RuntimeLogger.Log("FortuneAI started.");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "祈愿运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取祈愿运势",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty"
            )]
        [GroupEnterCommand(
            Command = ".luck",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每天运势",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty"
            )]
        public void RandomFortune(GroupMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.RandomFortune.Where(r => r.QQNum == MsgDTO.FromQQ);
                if (!query.IsNullOrEmpty())
                {
                    var f = query.First();
                    if (DateTime.Now.Date > f.UpdateDate.Date)
                    {
                        f.FortuneValue = GetRandomFortune();
                        f.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }

                    ShowRandFortune(MsgDTO, f);
                    return;
                }

                var randFor = GetRandomFortune();
                var rf = new RandomFortune
                {
                    Id = Guid.NewGuid().ToString(),
                    UpdateDate = DateTime.Now,
                    QQNum = MsgDTO.FromQQ,
                    FortuneValue = randFor
                };
                RandBless(rf);

                db.RandomFortune.Add(rf);
                db.SaveChanges();
                ShowRandFortune(MsgDTO, rf);
            }
        }

        private static void RandBless(RandomFortune rf)
        {
            using (var db = new AIDatabase())
            {
                var rand = new Random();
                if (rf.FortuneValue >= 50 || rand.Next(100) > 10) return;
                var filist = db.FortuneItem;
                var idx = rand.Next(filist.Count());
                var item = filist.OrderBy(p => p.Id).Skip(idx).First();
                rf.BlessName = item.Name;
                rf.BlessValue = item.Value;
            }
        }

        [GroupEnterCommand(
            Command = "星座运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取星座运势",
            Syntax = "[星座名]",
            Tag = "运势功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void StarFortune(GroupMsgDTO MsgDTO, object[] param)
        {
            var jr = new FortuneRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        public static void ReportCallBack(GroupMsgDTO MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = Report
            });
        }

        private static int GetRandomFortune()
        {
            var rand = new Random();
            return rand.Next(101);
        }

        private static void ShowRandFortune(GroupMsgDTO MsgDTO, RandomFortune rf)
        {
            var msg = string.Empty;

            if (rf.BlessValue > 0)
            {
                rf.FortuneValue = rf.FortuneValue + rf.BlessValue;
                msg += $"恭喜你收到了 {rf.BlessName} 的祝福\r";
                msg += $"你今天的运势是：{(rf.FortuneValue > 100 ? 100 : rf.FortuneValue)}%({rf.BlessValue}↑)\r";
            }
            else
            {
                msg += "你今天的运势是：" + rf.FortuneValue + "%\r";
            }
            var builder = new StringBuilder();
            builder.Append(msg);

            for (var i = 0; i < rf.FortuneValue; i++)
            {
                builder.Append("|");
            }
            msg = builder.ToString();

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = msg
            });
        }

        [GroupEnterCommand(
            Command = "塔罗牌占卜",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每日塔罗牌占卜",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty"
            )]
        [GroupEnterCommand(
            Command = ".zhan",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每日塔罗牌占卜",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty"
            )]
        public void TarotFortune(GroupMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.TarotFortuneRecord.Where(t => t.QQNum == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    var fortune = GetRandTarotFortune();
                    SendTarotFortune(MsgDTO, fortune);
                    db.TarotFortuneRecord.Add(new TarotFortuneRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        QQNum = MsgDTO.FromQQ,
                        UpdateTime = DateTime.Now.Date,
                        TarotId = fortune.Id
                    });
                    db.SaveChanges();
                    return;
                }

                var rec = query.First();
                if (rec.UpdateTime < DateTime.Now.Date)
                {
                    var fortune = GetRandTarotFortune();
                    SendTarotFortune(MsgDTO, fortune);
                    rec.UpdateTime = DateTime.Now.Date;
                    rec.TarotId = fortune.Id;
                    db.SaveChanges();
                    return;
                }

                var data = db.TarotFortuneData.First(p => p.Id == rec.TarotId);
                SendTarotFortune(MsgDTO, data);
            }
        }

        private void SendTarotFortune(GroupMsgDTO MsgDTO, TarotFortuneData data)
        {
            var msg = CodeApi.Code_Image(TarotServerPath + data.PicSrc) + '\r';
            msg += "牌名：" + data.Name + '\r';
            msg += data.IsPos ? "正位解释：" : "逆位解释：";
            msg += data.Description;

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = msg
            });
        }

        private static TarotFortuneData GetRandTarotFortune()
        {
            using (var db = new AIDatabase())
            {
                var datas = db.TarotFortuneData.OrderBy(p => p.Id);
                var count = datas.Count();

                var ran = new Random();
                var randData = datas.Skip(ran.Next(count)).First();
                return randData.Clone();
            }
        }

        [GroupEnterCommand(
            Command = "圣光祝福",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "祝福一个成员，让其随机运势增加80%（最高100%），当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At"
            )]
        public void HolyLight(GroupMsgDTO MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "圣光祝福", 80);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "祝福成功！"
            });
        }

        private void Bless(long QQNum, string BlessName, int BlessValue)
        {
            using (var db = new AIDatabase())
            {
                var query = db.RandomFortune.Where(h => h.QQNum == QQNum);
                if (query.IsNullOrEmpty())
                {
                    var randFor = GetRandomFortune();
                    var rf = new RandomFortune
                    {
                        Id = Guid.NewGuid().ToString(),
                        UpdateDate = DateTime.Now,
                        QQNum = QQNum,
                        FortuneValue = randFor,
                        BlessName = BlessName,
                        BlessValue = BlessValue
                    };
                    db.RandomFortune.Add(rf);
                }
                else
                {
                    var fortune = query.First();
                    fortune.BlessName = BlessName;
                    fortune.BlessValue = BlessValue;
                }

                db.SaveChanges();
            }
        }
    }
}