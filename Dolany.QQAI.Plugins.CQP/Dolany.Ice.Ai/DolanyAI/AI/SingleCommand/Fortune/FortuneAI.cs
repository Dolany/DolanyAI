using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;
using static Dolany.Ice.Ai.MahuaApis.CodeApi;

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
        private const string TarotServerPath = "https://m.sheup.com/";

        public FortuneAI()
        {
            RuntimeLogger.Log("FortuneAI started.");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = ".luck 祈愿运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每天运势",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = true
            )]
        public void RandomFortune(ReceivedMsgDTO MsgDTO, object[] param)
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
                        f.BlessName = "";
                        f.BlessValue = 0;
                        RandBless(f);

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
                    FortuneValue = randFor,
                    BlessName = "",
                    BlessValue = 0
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
                if (rf.FortuneValue >= 50 ||
                    Utility.RandInt(100) > 10)
                {
                    return;
                }
                var filist = db.FortuneItem;
                var idx = Utility.RandInt(filist.Count());
                var item = filist.OrderBy(p => p.Id)
                                 .Skip(idx)
                                 .First();
                rf.BlessName = item.Name;
                rf.BlessValue = item.Value;
            }
        }

        [EnterCommand(
            Command = "星座运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取星座运势",
            Syntax = "[星座名]",
            Tag = "运势功能",
            SyntaxChecker = "NotEmpty",
            IsPrivateAvailabe = true
            )]
        public void StarFortune(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var jr = new FortuneRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        private static void ReportCallBack(ReceivedMsgDTO MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(MsgDTO, Report);
        }

        private static int GetRandomFortune()
        {
            return Utility.RandInt(101);
        }

        private static void ShowRandFortune(ReceivedMsgDTO MsgDTO, RandomFortune rf)
        {
            var msg = string.Empty;

            if (rf.BlessValue > 0)
            {
                rf.FortuneValue = rf.FortuneValue + rf.BlessValue;
                msg += $"恭喜你受到了 {rf.BlessName} 的祝福\r";
                msg += $"你今天的运势是：{(rf.FortuneValue > 100 ? 100 : rf.FortuneValue)}%({rf.BlessValue}↑)\r";
            }
            else if (rf.BlessValue < 0)
            {
                rf.FortuneValue = rf.FortuneValue + rf.BlessValue;
                msg += $"哎呀呀，你受到了 {rf.BlessName} 的诅咒\r";
                msg += $"你今天的运势是：{(rf.FortuneValue < 0 ? 0 : rf.FortuneValue)}%({Math.Abs(rf.BlessValue)}↓)\r";
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

            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        [EnterCommand(
            Command = ".zhan 塔罗牌占卜",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每日塔罗牌占卜",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = true
            )]
        public void TarotFortune(ReceivedMsgDTO MsgDTO, object[] param)
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

        private static void SendTarotFortune(ReceivedMsgDTO MsgDTO, TarotFortuneData data)
        {
            var msg = Code_Image(TarotServerPath + data.PicSrc) + '\r';
            msg += "牌名：" + data.Name + '\r';
            msg += data.IsPos ? "正位解释：" : "逆位解释：";
            msg += data.Description;

            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        private static TarotFortuneData GetRandTarotFortune()
        {
            using (var db = new AIDatabase())
            {
                var datas = db.TarotFortuneData.OrderBy(p => p.Id);
                var count = datas.Count();

                var randData = datas.Skip(Utility.RandInt(count))
                                    .First();
                return randData.Clone();
            }
        }

        [EnterCommand(
            Command = "圣光祝福",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "祝福一个成员，让其随机运势增加80%（最高100%），当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At",
            IsPrivateAvailabe = false
            )]
        public void HolyLight(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "圣光祝福", 80);
            MsgSender.Instance.PushMsg(MsgDTO, "祝福成功！");
        }

        [EnterCommand(
            Command = "创世神祝福",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "祝福一个成员，让其随机运势增加100%，当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At",
            IsPrivateAvailabe = false
        )]
        public void CreatorBless(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "创世神", 100);
            MsgSender.Instance.PushMsg(MsgDTO, "祝福成功！");
        }

        private static void Bless(long QQNum, string BlessName, int BlessValue)
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

        [EnterCommand(
            Command = "暗夜诅咒",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "诅咒一个成员，让其随机运势减少若干点（最低0%），当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At",
            IsPrivateAvailabe = false
        )]
        public void Darkness(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "暗夜诅咒", -GetRandomFortune());
            MsgSender.Instance.PushMsg(MsgDTO, "诅咒成功！");
        }
    }
}