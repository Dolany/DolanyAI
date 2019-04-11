using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.SingleCommand.Fortune
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Base;

    using Cache;
    using Database.Sqlite;
    using Database.Sqlite.Model;

    using JetBrains.Annotations;

    using Model;

    [AI(
        Name = "麻将运势",
        Description = "AI for Getting Daily Maj Fortune.",
        Enable = true,
        PriorityLevel = 10)]
    public class MajFortuneAI : AIBase
    {
        private readonly string[] PosArray = { "东", "南", "西", "北" };

        private const string CharactorPath = "MajCharactor";

        private Dictionary<string, string> CharactorsDic { get; } = new Dictionary<string, string>();
        private Dictionary<string, int> KindDic = new Dictionary<string, int>();

        private int SumRate;

        public override void Initialization()
        {
            ReadCharactorsDic();
            ReadKindsList();
        }

        private void ReadCharactorsDic()
        {
            var dir = new DirectoryInfo(CharactorPath);
            foreach (var file in dir.GetFiles())
            {
                var name = file.Name.Split('.').First();
                CharactorsDic.Add(name, file.FullName);
            }
        }

        private void ReadKindsList()
        {
            KindDic = CommonUtil.ReadJsonData<Dictionary<string, int>>("majConfigData");
            SumRate = KindDic.Sum(p => p.Value);
        }

        [EnterCommand(
            Command = ".maj 麻将运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每天麻将运势",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool RandomMajFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var fortune = TodayFortune(MsgDTO.FromQQ);
            var msg = FortunePrintString(fortune);
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [NotNull]
        private string FortunePrintString(MajFortuneCache fortune)
        {
            var msg = "今日麻将运势：" + $"\r整体运势：{GetStarString(fortune.FortuneStar)} " + $"\r吉位：{fortune.Position} "
                      + $"\r牌运：{fortune.Kind} " + $"\r代表人物：{fortune.CharactorName} "
                      + $"\r{CodeApi.Code_Image(fortune.CharactorPath)}";

            return msg;
        }

        [NotNull]
        private MajFortuneCache TodayFortune(long QQNum)
        {
            var key = $"MajFortune-{QQNum}";
            var response = SCacheService.Get<MajFortuneCache>(key);

            if (response != null)
            {
                return response;
            }

            var newFortune = this.NewFortune(QQNum);
            SCacheService.Cache(key, newFortune);

            return newFortune;
        }

        [NotNull]
        private MajFortuneCache NewFortune(long QQNum)
        {
            var fortuneStar = CommonUtil.RandInt(11);
            var position = PosArray.RandElement();
            var kind = GetRandKind();
            var (key, value) = CharactorsDic.RandElement();

            return new MajFortuneCache
            {
                CharactorName = key,
                CharactorPath = value,
                FortuneStar = fortuneStar,
                Kind = kind,
                Position = position,
                QQNum = QQNum
            };
        }

        private string GetRandKind()
        {
            var rand = CommonUtil.RandInt(SumRate);
            var tempSum = 0;
            foreach (var (key, value) in KindDic)
            {
                if (tempSum + value > rand)
                {
                    return key;
                }

                tempSum += value;
            }

            return string.Empty;
        }

        [NotNull]
        private string GetStarString(int fortune)
        {
            var str = string.Empty;
            var stars = fortune / 2;
            for (var i = 0; i < stars; i++)
            {
                str += "★";
            }

            if (fortune % 2 == 1)
            {
                str += "☆";
            }

            return str;
        }
    }
}
