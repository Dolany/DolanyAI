using System;
using System.Linq;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Dolany.Database.Sqlite.Model;
using Dolany.Game.Alchemy;

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
            if (!player.CheckMatiral(bookItem.Item.CombineNeed, di, out var msg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"材料不足！\r{msg}");
                return false;
            }

            // do alchemy
            var table = AlTable.GetTable(MsgDTO.FromQQ);
            var result = table.DoAlchemy(player, di, bookItem.Item);
            MsgSender.Instance.PushMsg(MsgDTO, result.ToString(), true);

            // checkExam
            if (result.IsSuccess)
            {
                CheckExam(player, bookItem.Item.Name, bookItem.Level, MsgDTO);
            }

            // update person
            player.Update();
            di.Update();

            return true;
        }

        private void CheckExam(AlPlayer player, string itemName, int level, MsgInformationEx MsgDTO)
        {
            var cache = SCacheService.Get<MagicExamCache>($"Examing-{player.QQNum}");
            if (cache == null)
            {
                return;
            }

            var fullName = $"{itemName}lv{level}";
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
            
            return true;
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
                      $"考试将持续一个小时，期间需要你新炼制随机指定的三个最高等级的炼成品，是否确认参加考试？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "操作取消");
                return false;
            }

            var questions = MagicBookHelper.Instance.GetExamQuestions(player.MagicBookLearning, 3);
            msg = $"考试开始！这次考试的题目是：" +
                  $"{string.Join("，", questions.Select(q => $"{q.Name}lv{q.MaxLevel}"))}";
            MsgSender.Instance.PushMsg(MsgDTO, msg);

            cache = new MagicExamCache
            {
                EndTime = DateTime.Now.AddHours(1),
                Qustions = questions.ToDictionary(q => $"{q.Name}lv{q.MaxLevel}", q => false),
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
