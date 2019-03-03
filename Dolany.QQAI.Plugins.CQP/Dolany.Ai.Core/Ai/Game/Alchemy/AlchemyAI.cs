using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database.Ai;
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
                CheckExam(player);
            }

            // update person
            player.Update();
            di.Update();

            return true;
        }

        private void CheckExam(AlPlayer player)
        {
            // todo
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

        [EnterCommand(Command = "炼金考试",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "参加指定魔法书的炼金考试",
            Syntax = "[魔法书名]",
            SyntaxChecker = "Word",
            Tag = "炼金",
            IsPrivateAvailable = false,
            IsTesting = true)]
        public bool Exam(MsgInformationEx MsgDTO, object[] param)
        {
            
            return true;
        }
    }
}
