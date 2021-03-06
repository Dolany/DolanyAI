﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Sqlite.Model;
using Dolany.UtilityTool;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.Fortune
{
    public class FortuneAI : AIBase, IDataMgr
    {
        public override string AIName { get; set; } = "随机运势";

        public override string Description { get; set; } = "AI for Fortune.";

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.运势功能;

        private const string TarotServerPath = "./images/Tarot/";
        private string[] TarotSerialNames;

        private List<TarotFortuneDataModel> ModelList;

        private List<FortuneItemModel> FortuneItemList;

        public void RefreshData()
        {
            ModelList = CommonUtil.ReadJsonData_NamedList<TarotFortuneDataModel>("Standard/TarotFortuneData");
            FortuneItemList = CommonUtil.ReadJsonData_NamedList<FortuneItemModel>("Standard/FortuneItemData");

            var tarotFolder = new DirectoryInfo(TarotServerPath);
            if (tarotFolder.Exists)
            {
                TarotSerialNames = tarotFolder.GetDirectories().Select(p => p.Name).ToArray();
            }
        }

        [EnterCommand(ID = "FortuneAI_RandomFortune",
            Command = ".luck 祈愿运势 。luck",
            Description = "获取每天运势",
            IsPrivateAvailable = true)]
        public bool RandomFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var response = PersonCacheRecord.Get(MsgDTO.FromQQ, "RandomFortune");

            if (string.IsNullOrEmpty(response.Value))
            {
                var randFor = GetRandomFortune();
                var rf = new RandomFortuneCache {QQNum = MsgDTO.FromQQ, FortuneValue = randFor, BlessName = string.Empty, BlessValue = 0};
                RandBless(rf);
                ShowRandFortune(MsgDTO, rf);

                response.Value = JsonConvert.SerializeObject(rf);
                response.ExpiryTime = CommonUtil.UntilTommorow();
                response.Update();
            }
            else
            {
                ShowRandFortune(MsgDTO, JsonConvert.DeserializeObject<RandomFortuneCache>(response.Value));
            }
            return true;
        }

        private void RandBless(RandomFortuneCache rf)
        {
            if (rf.FortuneValue >= 50 || Rander.RandInt(100) > 20)
            {
                return;
            }

            var item = FortuneItemList.RandElement();
            rf.BlessName = item.Name;
            rf.BlessValue = item.Value;
        }

        [EnterCommand(ID = "FortuneAI_StarFortune",
            Command = "星座运势",
            Description = "获取星座运势",
            SyntaxHint = "[星座名]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool StarFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var jr = new FortuneRequestor(MsgDTO);
            jr.Work();
            return true;
        }

        private static int GetRandomFortune()
        {
            return Rander.RandInt(101);
        }

        private static void ShowRandFortune(MsgInformationEx MsgDTO, RandomFortuneCache rf)
        {
            var msg = string.Empty;

            if (rf.BlessValue > 0)
            {
                rf.FortuneValue += rf.BlessValue;
                rf.FortuneValue = rf.FortuneValue > 100 ? 100 : rf.FortuneValue;
                msg += $"恭喜你受到了 {rf.BlessName} 的祝福\r\n";
                msg += $"你今天的运势是：{rf.FortuneValue}%({rf.BlessValue}↑)\r\n";
            }
            else if (rf.BlessValue < 0)
            {
                rf.FortuneValue += rf.BlessValue;
                rf.FortuneValue = rf.FortuneValue < 0 ? 0 : rf.FortuneValue;
                msg += $"哎呀呀，你受到了 {rf.BlessName} 的诅咒\r\n";
                msg += $"你今天的运势是：{rf.FortuneValue}%({Math.Abs(rf.BlessValue)}↓)\r\n";
            }
            else
            {
                msg += "你今天的运势是：" + rf.FortuneValue + "%\r\n";
            }

            var builder = new StringBuilder();
            builder.Append(msg);

            for (var i = 0; i < rf.FortuneValue; i++)
            {
                builder.Append("|");
            }

            msg = builder.ToString();

            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        [EnterCommand(ID = "FortuneAI_TarotFortune",
            Command = ".zhan 塔罗牌占卜 。zhan",
            Description = "获取每日塔罗牌占卜",
            IsPrivateAvailable = true)]
        public bool TarotFortune(MsgInformationEx MsgDTO, object[] param)
        {
            var cache = PersonCacheRecord.Get(MsgDTO.FromQQ, "TarotFortune");

            TarotCacheModel fortune;
            if (string.IsNullOrEmpty(cache.Value))
            {
                fortune = GetRandTarotFortune();
                cache.Value = JsonConvert.SerializeObject(fortune);
                cache.ExpiryTime = CommonUtil.UntilTommorow();
                cache.Update();
            }
            else
            {
                fortune = JsonConvert.DeserializeObject<TarotCacheModel>(cache.Value);
            }

            SendTarotFortune(MsgDTO, fortune);
            return true;
        }

        private void SendTarotFortune(MsgInformationEx MsgDTO, TarotCacheModel data)
        {
            if (data == null)
            {
                return;
            }

            var ptr = data.IsPos ? "正位" : "逆位";
            var msg = $"{CodeApi.Code_Image_Relational($"{TarotServerPath}{data.SerialName}/{data.CardName}.jpg")}\r\n";
            msg += $"牌名：{ptr}{data.CardName}\r\n";
            msg += $"{ptr}解释：";
            var model = ModelList.First(p => p.Name == data.CardName);
            msg += data.IsPos ? model.正位 : model.逆位;

            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        private TarotCacheModel GetRandTarotFortune()
        {
            return new TarotCacheModel()
            {
                CardName = ModelList.RandElement().Name,
                IsPos = Rander.RandBool(),
                SerialName = TarotSerialNames.RandElement()
            };
        }

        [EnterCommand(ID = "FortuneAI_HolyLight",
            Command = "圣光祝福",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "祝福一个成员，让其随机运势增加80%（最高100%），当日有效",
            SyntaxHint = "[@qq号码]",
            SyntaxChecker = "At",
            DailyLimit = 5)]
        public bool HolyLight(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "圣光祝福", 80);
            MsgSender.PushMsg(MsgDTO, "祝福成功！");
            return true;
        }

        [EnterCommand(ID = "FortuneAI_CreatorBless",
            Command = "创世神祝福",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "祝福一个成员，让其随机运势增加100%，当日有效",
            SyntaxHint = "[@qq号码]",
            SyntaxChecker = "At",
            DailyLimit = 3)]
        public bool CreatorBless(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "创世神", 100);
            MsgSender.PushMsg(MsgDTO, "祝福成功！");
            return true;
        }

        private static void Bless(long QQNum, string BlessName, int BlessValue)
        {
            var response = PersonCacheRecord.Get(QQNum, "RandomFortune");

            if (string.IsNullOrEmpty(response.Value))
            {
                var randFor = GetRandomFortune();
                var rf = new RandomFortuneCache()
                {
                    QQNum = QQNum,
                    FortuneValue = randFor,
                    BlessName = BlessName,
                    BlessValue = BlessValue
                };
                response.Value = JsonConvert.SerializeObject(rf);
                response.ExpiryTime = CommonUtil.UntilTommorow();
                response.Update();
            }
            else
            {
                var model = JsonConvert.DeserializeObject<RandomFortuneCache>(response.Value);
                model.BlessName = BlessName;
                model.BlessValue = BlessValue;

                response.Value = JsonConvert.SerializeObject(model);
                response.ExpiryTime = CommonUtil.UntilTommorow();
                response.Update();
            }
        }

        [EnterCommand(ID = "FortuneAI_Darkness",
            Command = "暗夜诅咒",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "诅咒一个成员，让其随机运势减少若干点（最低0%），当日有效",
            SyntaxHint = "[@qq号码]",
            SyntaxChecker = "At",
            DailyLimit = 3)]
        public bool Darkness(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long)param[0];

            Bless(aimNum, "暗夜诅咒", -GetRandomFortune());
            MsgSender.PushMsg(MsgDTO, "诅咒成功！");
            return true;
        }
    }

    public class TarotFortuneDataModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string 正位 { get; set; }

        public string 逆位 { get; set; }
    }

    public class FortuneItemModel : INamedJsonModel
    {
        public string Name { get; set; }
        public string Description { get; set;}
        public int Value { get; set; }
        public int Type { get; set; }
    }
}
