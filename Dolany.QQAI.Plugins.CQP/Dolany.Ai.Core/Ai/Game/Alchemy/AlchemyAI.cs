using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Dolany.Database.Sqlite.Model;
using Dolany.Game.Alchemy;
using Dolany.Game.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Alchemy
{
    [AI(Name = "炼金",
        Description = "AI for alchemy.",
        Enable = false,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class AlchemyAI : AIBase
    {
        [EnterCommand(Command = "炼成",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "炼成某个炼成品",
            Syntax = "[炼成品的名称]",
            SyntaxChecker = "Word",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool Alchemy(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            // check alive state
            var player = AlPlayer.GetPlayer(MsgDTO.FromQQ);
            if (!player.IsAlive)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你当前处于昏迷中，无法完成该操作！");
                return false;
            }

            // check name exist
            var bookItem = MagicBookHelper.Instance.FindItem(name);
            if (bookItem == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未找到相应的炼成品！");
                return false;
            }

            // check whether have learned
            var bookName = bookItem.MagicBook.Name;
            if (!player.MagicBookLearned.Contains(bookName) && player.MagicBookLearning != bookName)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你尚未学习该炼成品的炼成方法！");
                return false;
            }

            // check enough material
            var di = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (!bookItem.Item.CombineNeed.CheckNeed(player, osPerson, di, out var msg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"材料不足！\r{msg}");
                return false;
            }

            // do alchemy
            var table = AlTable.GetTable(MsgDTO.FromQQ);
            var result = table.DoAlchemy(player, di, bookItem.Item, osPerson);
            MsgSender.Instance.PushMsg(MsgDTO, result.ToString(), true);

            // checkExam
            if (result.IsSuccess)
            {
                CheckExam(player, bookItem.Item.Name, MsgDTO);
            }

            // update person
            player.Update();
            di.Update();
            osPerson.Update();

            return true;
        }

        private void CheckExam(AlPlayer player, string itemName, MsgInformationEx MsgDTO)
        {
            var cache = SCacheService.Get<MagicExamCache>($"Examing-{player.QQNum}");
            if (cache == null)
            {
                return;
            }

            var fullName = $"{itemName}";
            if (!cache.Qustions.ContainsKey(fullName))
            {
                return;
            }

            cache.Qustions[fullName] = true;
            if (!cache.IsPassed)
            {
                SCacheService.Cache($"Examing-{player.QQNum}", cache, cache.EndTime);
                return;
            }

            SCacheService.Cache($"Examing-{player.QQNum}", cache, DateTime.Now);
            MsgSender.Instance.PushMsg(MsgDTO, $"恭喜你完成了 {cache.BookName} 考试！");

            player.MagicBookLearning = string.Empty;
            player.MagicBookLearned.Add(cache.BookName);
        }

        [EnterCommand(Command = "我的魔法书",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的魔法书",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool MyMagicBook(MsgInformationEx MsgDTO, object[] param)
        {
            var player = AlPlayer.GetPlayer(MsgDTO.FromQQ);
            var msg = $"学习中：{player.MagicBookLearning}\r";
            msg += $"已学会：{string.Join("，", player.MagicBookLearned)}\r";
            msg += $"待学习：{string.Join("，", player.MagicBookAvailable)}";
            
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);

            return true;
        }

        [EnterCommand(Command = "学习魔法书",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "学习一本新的魔法书",
            Syntax = "[魔法书名]",
            SyntaxChecker = "Word",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool LearnMagicBook(MsgInformationEx MsgDTO, object[] param)
        {
            var bookName = param[0] as string;

            var player = AlPlayer.GetPlayer(MsgDTO.FromQQ);
            if (!string.IsNullOrEmpty(player.MagicBookLearning))
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"你正在学习 {player.MagicBookLearning} 中！");
                return false;
            }

            if (player.MagicBookLearned.Contains(bookName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你学会该魔法书啦！");
                return false;
            }

            if (!player.MagicBookAvailable.Contains(bookName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有获得该魔法书！");
                return false;
            }

            player.MagicBookAvailable.Remove(bookName);
            player.MagicBookLearning = bookName;
            player.Update();

            MsgSender.Instance.PushMsg(MsgDTO, $"开始学习 {bookName} 啦！要努力加油噢~");
            return true;
        }

        [EnterCommand(Command = "我的炼成台",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的炼成台信息",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool MyTable(MsgInformationEx MsgDTO, object[] param)
        {
            var table = AlTable.GetTable(MsgDTO.FromQQ);
            MsgSender.Instance.PushMsg(MsgDTO, table.ToString(), true);

            return true;
        }

        [EnterCommand(Command = "升级炼成台",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "升级炼成台，提高魔纹的上限值",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool UpgradeTable(MsgInformationEx MsgDTO, object[] param)
        {
            var table = AlTable.GetTable(MsgDTO.FromQQ);
            if (!table.CanUpgrade)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你需要将至少三个魔纹升至当前等级的最大值！");
                return false;
            }

            if (table.IsMaxLevel)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的炼成台已经满级！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var record = DriftItemRecord.GetRecord(MsgDTO.FromQQ);
            var player = AlPlayer.GetPlayer(MsgDTO.FromQQ);

            var LevelDataModel = AlTableHelper.Instance[table.Level];
            if (!LevelDataModel.UpgradeNeed.CheckNeed(player, osPerson, record, out var msg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"材料不足！\r{msg}");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, $"升级需要：\r{msg}\r是否确认升级？"))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "操作取消");
                return false;
            }

            LevelDataModel.UpgradeNeed.DoConsume(player, osPerson, record);
            table.Level++;

            player.Update();
            record.Update();
            osPerson.Update();
            table.Update();

            MsgSender.Instance.PushMsg(MsgDTO, $"升级成功！");

            return true;
        }

        [EnterCommand(Command = "查看魔法书",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看某本魔法书的详情",
            Syntax = "[魔法书名]",
            SyntaxChecker = "Word",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool ViewMagicBook(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var book = MagicBookHelper.Instance[name];
            if (book == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未查找到该魔法书！");
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, book.ToString());
            return true;
        }

        [EnterCommand(Command = "查看炼成品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看某件炼成品的详情",
            Syntax = "[炼成品名]",
            SyntaxChecker = "Word",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool ViewAlchemyItem(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var bookItem = MagicBookHelper.Instance.FindItem(name);
            if (bookItem == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未找到该炼成品！");
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, $"{bookItem.Item}\r《{bookItem.MagicBook.Name}》");
            return true;
        }

        [EnterCommand(Command = "我的炼成品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己拥有的炼成品",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool MyAlchemyItem(MsgInformationEx MsgDTO, object[] param)
        {
            var player = AlPlayer.GetPlayer(MsgDTO.FromQQ);
            if (player.AlItems.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有任何炼成品！");
                return false;
            }

            var items = player.AlItems.Select(it => new {Name = it.Key, Count = it.Value}).OrderBy(it => it.Name);

            var msg = $"你的炼成品有：\r{string.Join(",", items.Select(it => $"{it.Name}*{it.Count}"))}";
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(Command = "我的魔尘",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己拥有的魔尘",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool MyMagicDirt(MsgInformationEx MsgDTO, object[] param)
        {
            var player = AlPlayer.GetPlayer(MsgDTO.FromQQ);
            if (player.MagicDirt.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有任何魔尘！");
                return false;
            }

            var dirts = player.MagicDirt.Select(md => new {Name = md.Key, Count = md.Value});
            var msg = $"你的魔尘有：\r{string.Join(",", dirts.Select(md => $"{md.Name}*{md.Count}"))}";
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);

            return true;
        }

        [EnterCommand(Command = "ua",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "对某个目标使用某个炼成品",
            Syntax = "[炼成品名] [@qq号]",
            SyntaxChecker = "Word At",
            Tag = "炼金",
            IsPrivateAvailable = false,
            IsTesting = true)]
        public bool UseAlcheyItem(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var aimQQ = (long) param[1];

            UA(MsgDTO.FromQQ, aimQQ, name, MsgDTO.FromGroup);
            return true;
        }

        [EnterCommand(Command = "ua",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "对自己使用某个炼成品",
            Syntax = "[炼成品名]",
            SyntaxChecker = "Word",
            Tag = "炼金",
            IsPrivateAvailable = false,
            IsTesting = true)]
        public bool UseAlcheyItemToSelf(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;

            UA(MsgDTO.FromQQ, MsgDTO.FromQQ, name, MsgDTO.FromGroup);
            return true;
        }

        private void UA(long sourceQQ, long aimQQ, string name, long GroupNum)
        {
            var bookItem = MagicBookHelper.Instance.FindItem(name);
            if (bookItem == null)
            {
                MsgSender.Instance.PushMsg(GroupNum, sourceQQ, "未找到该炼成品！");
                return;
            }

            var sourcePlayer = AlPlayer.GetPlayer(sourceQQ);
            if (!sourcePlayer.AlItems.ContainsKey(name))
            {
                MsgSender.Instance.PushMsg(GroupNum, sourceQQ, "你没有该炼成品！");
                return;
            }

            var aimPlayer = sourceQQ == aimQQ ? sourcePlayer : AlPlayer.GetPlayer(aimQQ);
            bookItem.Item.DoEffect(sourcePlayer, aimPlayer, GroupNum);
        }

        [EnterCommand(Command = "报名考试",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "参加当前学习的魔法书的炼金考试",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "炼金",
            IsPrivateAvailable = false,
            IsTesting = true,
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool Exam(MsgInformationEx MsgDTO, object[] param)
        {
            var cache = SCacheService.Get<MagicExamCache>($"Examing-{MsgDTO.FromQQ}");
            if (cache != null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你正处于一场考试中！");
                return false;
            }

            var player = AlPlayer.GetPlayer(MsgDTO.FromQQ);
            if (string.IsNullOrEmpty(player.MagicBookLearning))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你当前没有学习任何魔法书！");
                return false;
            }

            var msg = $"你当前参加的是 {player.MagicBookLearning} 的考试。" +
                      $"考试将持续一个小时，期间需要你新炼制随机指定的三个炼成品，是否确认参加考试？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "操作取消");
                return false;
            }

            var questions = MagicBookHelper.Instance.GetExamQuestions(player.MagicBookLearning, 3);
            msg = $"考试开始！这次考试的题目是：" +
                  $"{string.Join("，", questions.Select(q => $"{q.Name}"))}";
            MsgSender.Instance.PushMsg(MsgDTO, msg);

            cache = new MagicExamCache
            {
                EndTime = DateTime.Now.AddHours(1),
                Qustions = questions.ToDictionary(q => $"{q.Name}", q => false),
                BookName = player.MagicBookLearning
            };
            SCacheService.Cache($"Examing-{MsgDTO.FromQQ}", cache, cache.EndTime);

            return true;
        }

        [EnterCommand(Command = "考试进度",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的考试进度",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "炼金",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public bool MyExaming(MsgInformationEx MsgDTO, object[] param)
        {
            var cache = SCacheService.Get<MagicExamCache>($"Examing-{MsgDTO.FromQQ}");
            if (cache == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你当前没有处于一场考试中！");
                return false;
            }

            var msg = string.Join("\r", cache.Qustions.Select(q => $"{q.Key}({(q.Value ? "√" : "×")})"));
            MsgSender.Instance.PushMsg(MsgDTO, $"你当前的考试进度为：\r{msg}", true);
            return true;
        }
    }
}
