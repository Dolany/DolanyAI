using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Sqlite.Model;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.Fortune
{
    public class MajFortuneAI : AIBase, IDataMgr
    {
        public override string AIName { get; set; } = "麻将运势";

        public override string Description { get; set; } = "AI for Getting Daily Maj Fortune.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        private readonly string[] PosArray = { "东", "南", "西", "北" };

        private const string CharactorPath = "MajCharactor";

        private Dictionary<string, string> CharactorsDic { get; } = new Dictionary<string, string>();
        private Dictionary<string, int> KindDic = new Dictionary<string, int>();

        private int SumRate;

        public override void Initialization()
        {
            ReadCharactorsDic();
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

        public void RefreshData()
        {
            KindDic = CommonUtil.ReadJsonData<Dictionary<string, int>>("majConfigData");
            SumRate = KindDic.Sum(p => p.Value);
        }

        [EnterCommand(ID = "MajFortuneAI_RandomMajFortune",
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
            var msg = "今日麻将运势：" + $"\r整体运势：{Utility.LevelToStars(fortune.FortuneStar)} " + $"\r吉位：{fortune.Position} "
                      + $"\r牌运：{fortune.Kind} " + $"\r代表人物：{fortune.CharactorName} "
                      + $"\r{CodeApi.Code_Image(fortune.CharactorPath)}";

            return msg;
        }

        [NotNull]
        private MajFortuneCache TodayFortune(long QQNum)
        {
            var cache = PersonCacheRecord.Get(QQNum, "MajFortune");

            if (!string.IsNullOrEmpty(cache.Value))
            {
                return JsonConvert.DeserializeObject<MajFortuneCache>(cache.Value);
            }

            var newFortune = this.NewFortune(QQNum);
            cache.Value = JsonConvert.SerializeObject(newFortune);
            cache.ExpiryTime = CommonUtil.UntilTommorow();
            cache.Update();

            return newFortune;
        }

        [NotNull]
        private MajFortuneCache NewFortune(long QQNum)
        {
            var fortuneStar = Rander.RandInt(11);
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
            var rand = Rander.RandInt(SumRate);
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
    }
}
