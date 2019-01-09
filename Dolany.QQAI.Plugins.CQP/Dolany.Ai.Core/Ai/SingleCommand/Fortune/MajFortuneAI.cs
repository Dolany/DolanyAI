namespace Dolany.Ai.Core.Ai.SingleCommand.Fortune
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Dolany.Ai.Core.API;
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Model;
    using Dolany.Database.Ai;

    using JetBrains.Annotations;

    [AI(
        Name = nameof(MajFortuneAI),
        Description = "AI for Getting Daily Maj Fortune.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class MajFortuneAI : AIBase
    {
        private readonly string[] PosArray = { "东", "南", "西", "北" };

        private const string MajConfigFile = "majConfig.txt";

        private const string CharactorPath = "MajCharactor";

        private Dictionary<string, string> CharactorsDic { get; } = new Dictionary<string, string>();
        private List<string> KindsList { get; } = new List<string>();

        public override void Work()
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
            var file = new FileInfo(MajConfigFile);
            using (var reader = new StreamReader(file.FullName))
            {
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    KindsList.Add(line);
                }
            }
        }

        [EnterCommand(
            Command = ".maj 麻将运势",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取每天麻将运势",
            Syntax = "",
            Tag = "运势功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false)]
        public void RandomMajFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var fortune = TodayFortune(MsgDTO.FromQQ);
            var msg = FortunePrintString(fortune);
            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        [NotNull]
        private string FortunePrintString(MajFortune fortune)
        {
            var msg = "今日麻将运势：" + $"\r整体运势：{GetStarString(fortune.FortuneStar)} " + $"\r吉位：{fortune.Position} "
                      + $"\r牌运：{fortune.Kind} " + $"\r代表人物：{fortune.CharactorName} "
                      + $"\r{CodeApi.Code_Image(fortune.CharactorPath)}";

            return msg;
        }

        [NotNull]
        private MajFortune TodayFortune(long QQNum)
        {
            using (var db = new AIDatabase())
            {
                var today = DateTime.Now.Date;
                var query = db.MajFortune.Where(m => m.QQNum == QQNum && m.UpdateTime == today).ToList();
                if (!query.IsNullOrEmpty())
                {
                    return query.First().Clone();
                }

                query = db.MajFortune.Where(m => m.QQNum == QQNum).ToList();
                if (query.IsNullOrEmpty())
                {
                    var newFortune = this.NewFortune(QQNum);
                    db.MajFortune.Add(newFortune);
                    db.SaveChanges();

                    return newFortune.Clone();
                }

                var fortune = query.First();

                var newFortuen = NewFortune(fortune.QQNum);

                fortune.CharactorName = newFortuen.CharactorName;
                fortune.CharactorPath = newFortuen.CharactorPath;
                fortune.FortuneStar = newFortuen.FortuneStar;
                fortune.Kind = newFortuen.Kind;
                fortune.Position = newFortuen.Position;
                fortune.UpdateTime = DateTime.Now.Date;

                db.SaveChanges();

                return fortune.Clone();
            }
        }

        [NotNull]
        private MajFortune NewFortune(long QQNum)
        {
            var fortuneStar = Utility.RandInt(11);
            var position = PosArray[Utility.RandInt(PosArray.Length)];
            var kind = KindsList[Utility.RandInt(KindsList.Count)];
            var dicIdx = Utility.RandInt(CharactorsDic.Count);
            var charactorName = CharactorsDic.Keys.ElementAt(dicIdx);
            var charactorPath = CharactorsDic.Values.ElementAt(dicIdx);

            return new MajFortune
                       {
                           CharactorName = charactorName,
                           CharactorPath = charactorPath,
                           FortuneStar = fortuneStar,
                           Kind = kind,
                           Position = position,
                           QQNum = QQNum,
                           UpdateTime = DateTime.Now.Date
                       };
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
