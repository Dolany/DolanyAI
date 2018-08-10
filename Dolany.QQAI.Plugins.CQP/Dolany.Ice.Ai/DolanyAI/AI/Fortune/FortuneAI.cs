using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = "FortuneAI",
        Description = "AI for Fortune.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class FortuneAI : AIBase
    {
        private string TarotServerPath = "https://m.sheup.com/";

        public FortuneAI()
            : base()
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
            using (AIDatabase db = new AIDatabase())
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

                int randFor = GetRandomFortune();
                var rf = new RandomFortune
                {
                    Id = Guid.NewGuid().ToString(),
                    UpdateDate = DateTime.Now,
                    QQNum = MsgDTO.FromQQ,
                    FortuneValue = randFor
                };
                db.RandomFortune.Add(rf);
                db.SaveChanges();
                ShowRandFortune(MsgDTO, rf);
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
            FortuneRequestor jr = new FortuneRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        public void ReportCallBack(GroupMsgDTO MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = Report
            });
        }

        private int GetRandomFortune()
        {
            Random rand = new Random();
            return rand.Next(101);
        }

        private void ShowRandFortune(GroupMsgDTO MsgDTO, RandomFortune rf)
        {
            string msg = string.Empty;
            Random rand = new Random();
            if (IsBlessed(MsgDTO.FromQQ))
            {
                rf.FortuneValue = rf.FortuneValue + 80;
                msg += $"恭喜你收到了圣光祝福\r";
                msg += "你今天的运势是：" + (rf.FortuneValue > 100 ? 100 : rf.FortuneValue) + "%(80↑)\r";
            }
            else if (rf.FortuneValue < 50 && rand.Next(100) <= 30)
            {
                using (AIDatabase db = new AIDatabase())
                {
                    var query = db.FortuneItem;
                    int idx = rand.Next(query.Count());
                    var item = query.OrderBy(p => p.Id).Skip(idx).First();
                    rf.FortuneValue += item.Value;
                    rf.FortuneValue = rf.FortuneValue > 100 ? 100 : rf.FortuneValue;
                    msg += $"恭喜你收到了 {item.Name} 的祝福\r";
                    msg += $"你今天的运势是：{rf.FortuneValue}%({item.Value}↑)\r";
                }
            }
            else
            {
                msg += "你今天的运势是：" + rf.FortuneValue + "%\r";
            }

            for (int i = 0; i < rf.FortuneValue; i++)
            {
                msg += "|";
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = msg
            });
        }

        private bool IsBlessed(long QQNum)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.HolyLightBless.Where(p => p.QQNum == QQNum);
                if (query.IsNullOrEmpty())
                {
                    return false;
                }

                var bless = query.First();
                if (bless.BlessDate < DateTime.Now.Date)
                {
                    return false;
                }

                return true;
            }
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
            using (AIDatabase db = new AIDatabase())
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
            string msg = CodeApi.Code_Image(TarotServerPath + data.PicSrc) + '\r';
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

        private TarotFortuneData GetRandTarotFortune()
        {
            using (AIDatabase db = new AIDatabase())
            {
                var datas = db.TarotFortuneData.OrderBy(p => p.Id);
                int count = datas.Count();

                Random ran = new Random();
                var randData = datas.Skip(ran.Next(count)).First();
                return randData.Clone();
            }
        }

        [GroupEnterCommand(
            Command = "圣光祝福",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "祝福一个成员，让其随机运势增加80%（最高100%），当日有效",
            Syntax = "[@qq号码]",
            Tag = "运势功能",
            SyntaxChecker = "At"
            )]
        public void HolyLight(GroupMsgDTO MsgDTO, object[] param)
        {
            long aimNum = (long)param[0];
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.HolyLightBless.Where(h => h.QQNum == aimNum);
                if (query.IsNullOrEmpty())
                {
                    db.HolyLightBless.Add(new HolyLightBless
                    {
                        Id = Guid.NewGuid().ToString(),
                        QQNum = aimNum,
                        BlessDate = DateTime.Now.Date
                    });
                }
                else
                {
                    var bless = query.First();
                    bless.BlessDate = DateTime.Now.Date;
                }

                db.SaveChanges();
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = "祝福成功！"
                });
            }
        }
    }
}