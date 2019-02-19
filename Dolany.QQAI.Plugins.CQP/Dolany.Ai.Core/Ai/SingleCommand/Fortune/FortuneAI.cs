using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Ai.SingleCommand.Fortune
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using API;

    using Base;

    using Cache;

    using Database;
    using Dolany.Database.Ai;
    using Database.Sqlite.Model;

    using Model;

    [AI(
        Name = "随机运势",
        Description = "AI for Fortune.",
        Enable = true,
        PriorityLevel = 10)]
    public class FortuneAI : AIBase
    {
        private const string TarotServerPath = "https://m.sheup.com/";
        private List<TarotFortuneDataModel> DataList;

        public override void Initialization()
        {
            var dataDic = CommonUtil.ReadJsonData<Dictionary<string, TarotFortuneDataModel>>("TarotFortuneData");
            DataList = dataDic.Select(d =>
            {
                var (key, value) = d;
                value.Name = key;
                return value;
            }).ToList();
        }

        [EnterCommand(
            Command = ".luck 祈愿运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每天运势",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool RandomFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var key = $"RandomFortune-{MsgDTO.FromQQ}";
            var response = SCacheService.Get<RandomFortuneCache>(key);

            if (response == null)
            {
                var randFor = GetRandomFortune();
                var rf = new RandomFortuneCache
                {
                                 QQNum = MsgDTO.FromQQ,
                                 FortuneValue = randFor,
                                 BlessName = string.Empty,
                                 BlessValue = 0
                             };
                RandBless(rf);
                ShowRandFortune(MsgDTO, rf);

                SCacheService.Cache(key, rf);
            }
            else
            {
                ShowRandFortune(MsgDTO, response);
            }
            return true;
        }

        private static void RandBless(RandomFortuneCache rf)
        {
            if (rf.FortuneValue >= 50 || CommonUtil.RandInt(100) > 10)
            {
                return;
            }

            var filist = MongoService<FortuneItem>.Get();
            var idx = CommonUtil.RandInt(filist.Count());
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
        public bool StarFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var jr = new FortuneRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
            return true;
        }

        private static void ReportCallBack(MsgInformationEx MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(MsgDTO, Report);
        }

        private static int GetRandomFortune()
        {
            return CommonUtil.RandInt(101);
        }

        private static void ShowRandFortune(MsgInformationEx MsgDTO, RandomFortuneCache rf)
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

            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }

        [EnterCommand(
            Command = ".zhan 塔罗牌占卜",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每日塔罗牌占卜",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool TarotFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var key = $"TarotFortune-{MsgDTO.FromQQ}";
            var cache = SCacheService.Get<string>(key);

            TarotFortuneDataModel fortune;
            if (string.IsNullOrEmpty(cache))
            {
                fortune = GetRandTarotFortune();
                SCacheService.Cache(key, fortune.Name);
            }
            else
            {
                fortune = DataList.FirstOrDefault(d => d.Name == cache);
            }

            SendTarotFortune(MsgDTO, fortune);
            return true;
        }

        private static void SendTarotFortune(MsgInformationEx MsgDTO, TarotFortuneDataModel data)
        {
            if (data == null)
            {
                return;
            }

            var msg = CodeApi.Code_Image(TarotServerPath + data.PicSrc) + '\r';
            msg += "牌名：" + data.Name + '\r';
            msg += data.IsPos ? "正位解释：" : "逆位解释：";
            msg += data.Description;

            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }

        private TarotFortuneDataModel GetRandTarotFortune()
        {
            var count = DataList.Count;

            return DataList[CommonUtil.RandInt(count)];
        }

        [EnterCommand(
            Command = "圣光祝福",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "祝福一个成员，让其随机运势增加80%（最高100%），当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 5)]
        public bool HolyLight(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "圣光祝福", 80);
            MsgSender.Instance.PushMsg(MsgDTO, "祝福成功！");
            return true;
        }

        [EnterCommand(
            Command = "创世神祝福",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "祝福一个成员，让其随机运势增加100%，当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 3)]
        public bool CreatorBless(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "创世神", 100);
            MsgSender.Instance.PushMsg(MsgDTO, "祝福成功！");
            return true;
        }

        private static void Bless(long QQNum, string BlessName, int BlessValue)
        {
            var key = $"RandomFortune-{QQNum}";
            var response = SCacheService.Get<RandomFortuneCache>(key);

            if (response == null)
            {
                var randFor = GetRandomFortune();
                var rf = new RandomFortuneCache()
                {
                    QQNum = QQNum,
                    FortuneValue = randFor,
                    BlessName = BlessName,
                    BlessValue = BlessValue
                };
                SCacheService.Cache(key, rf);
            }
            else
            {
                response.BlessName = BlessName;
                response.BlessValue = BlessValue;

                SCacheService.Cache(key, response);
            }
        }

        [EnterCommand(
            Command = "暗夜诅咒",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "诅咒一个成员，让其随机运势减少若干点（最低0%），当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 3)]
        public bool Darkness(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "暗夜诅咒", -GetRandomFortune());
            MsgSender.Instance.PushMsg(MsgDTO, "诅咒成功！");
            return true;
        }
    }

    public class TarotFortuneDataModel
    {
        public string Name { get; set; }
        public bool IsPos { get; set; }
        public string Description { get; set; }
        public string PicSrc { get; set; }
    }
}
