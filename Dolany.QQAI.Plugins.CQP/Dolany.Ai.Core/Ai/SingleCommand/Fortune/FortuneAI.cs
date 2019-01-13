namespace Dolany.Ai.Core.Ai.SingleCommand.Fortune
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using API;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Dolany.Database;
    using Dolany.Database.Ai;

    using Model;

    [AI(
        Name = nameof(FortuneAI),
        Description = "AI for Fortune.",
        IsAvailable = true,
        PriorityLevel = 10)]
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
            IsPrivateAvailable = true)]
        public void RandomFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var query = MongoService<RandomFortune>.Get(r => r.QQNum == MsgDTO.FromQQ);
            if (!query.IsNullOrEmpty())
            {
                var f = query.First();
                if (DateTime.Now.Date > f.UpdateDate.Date)
                {
                    f.FortuneValue = GetRandomFortune();
                    f.UpdateDate = DateTime.Now;
                    f.BlessName = string.Empty;
                    f.BlessValue = 0;
                    RandBless(f);

                    MongoService<RandomFortune>.Update(f);
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
                BlessName = string.Empty,
                BlessValue = 0
            };
            RandBless(rf);

            MongoService<RandomFortune>.Insert(rf);
            ShowRandFortune(MsgDTO, rf);
        }

        private static void RandBless(RandomFortune rf)
        {
            if (rf.FortuneValue >= 50 ||
                Utility.RandInt(100) > 10)
            {
                return;
            }

            var filist = MongoService<FortuneItem>.Get();
            var idx = Utility.RandInt(filist.Count());
            var item = filist.OrderBy(p => p.Id)
                .Skip(idx)
                .First();
            rf.BlessName = item.Name;
            rf.BlessValue = item.Value;
        }

        [EnterCommand(
            Command = "星座运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取星座运势",
            Syntax = "[星座名]",
            Tag = "运势功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public void StarFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var jr = new FortuneRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        private static void ReportCallBack(MsgInformationEx MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(MsgDTO, Report);
        }

        private static int GetRandomFortune()
        {
            return Utility.RandInt(101);
        }

        private static void ShowRandFortune(MsgInformationEx MsgDTO, RandomFortune rf)
        {
            var msg = string.Empty;

            if (rf.BlessValue > 0)
            {
                rf.FortuneValue = rf.FortuneValue + rf.BlessValue;
                rf.FortuneValue = rf.FortuneValue > 100 ? 100 : rf.FortuneValue;
                msg += $"恭喜你受到了 {rf.BlessName} 的祝福\r";
                msg += $"你今天的运势是：{rf.FortuneValue}%({rf.BlessValue}↑)\r";
            }
            else if (rf.BlessValue < 0)
            {
                rf.FortuneValue = rf.FortuneValue + rf.BlessValue;
                rf.FortuneValue = rf.FortuneValue < 0 ? 0 : rf.FortuneValue;
                msg += $"哎呀呀，你受到了 {rf.BlessName} 的诅咒\r";
                msg += $"你今天的运势是：{rf.FortuneValue}%({Math.Abs(rf.BlessValue)}↓)\r";
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
            IsPrivateAvailable = true)]
        public void TarotFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var query = MongoService<TarotFortuneRecord>.Get(t => t.QQNum == MsgDTO.FromQQ);
            if (query.IsNullOrEmpty())
            {
                var fortune = GetRandTarotFortune();
                SendTarotFortune(MsgDTO, fortune);
                MongoService<TarotFortuneRecord>.Insert(new TarotFortuneRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    QQNum = MsgDTO.FromQQ,
                    UpdateTime = DateTime.Now.Date,
                    TarotId = fortune.Id
                });

                return;
            }

            var rec = query.First();
            if (rec.UpdateTime < DateTime.Now.Date)
            {
                var fortune = GetRandTarotFortune();
                SendTarotFortune(MsgDTO, fortune);
                rec.UpdateTime = DateTime.Now.Date;
                rec.TarotId = fortune.Id;
                MongoService<TarotFortuneRecord>.Update(rec);

                return;
            }

            var data = MongoService<TarotFortuneData>.Get(p => p.Id == rec.TarotId).First();
            SendTarotFortune(MsgDTO, data);
        }

        private static void SendTarotFortune(MsgInformationEx MsgDTO, TarotFortuneData data)
        {
            var msg = CodeApi.Code_Image(TarotServerPath + data.PicSrc) + '\r';
            msg += "牌名：" + data.Name + '\r';
            msg += data.IsPos ? "正位解释：" : "逆位解释：";
            msg += data.Description;

            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        private static TarotFortuneData GetRandTarotFortune()
        {
            var datas = MongoService<TarotFortuneData>.Get().OrderBy(p => p.Id).ToList();
            var count = datas.Count();

            var randData = datas.Skip(Utility.RandInt(count))
                .First();
            return randData.Clone();
        }

        [EnterCommand(
            Command = "圣光祝福",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "祝福一个成员，让其随机运势增加80%（最高100%），当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false)]
        public void HolyLight(MsgInformationEx MsgDTO, object[] param)
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
            IsPrivateAvailable = false)]
        public void CreatorBless(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "创世神", 100);
            MsgSender.Instance.PushMsg(MsgDTO, "祝福成功！");
        }

        private static void Bless(long QQNum, string BlessName, int BlessValue)
        {
            var query = MongoService<RandomFortune>.Get(h => h.QQNum == QQNum);
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
                MongoService<RandomFortune>.Insert(rf);
            }
            else
            {
                var fortune = query.First();
                fortune.BlessName = BlessName;
                fortune.BlessValue = BlessValue;

                MongoService<RandomFortune>.Update(fortune);
            }
        }

        [EnterCommand(
            Command = "暗夜诅咒",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "诅咒一个成员，让其随机运势减少若干点（最低0%），当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false)]
        public void Darkness(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "暗夜诅咒", -GetRandomFortune());
            MsgSender.Instance.PushMsg(MsgDTO, "诅咒成功！");
        }
    }
}
