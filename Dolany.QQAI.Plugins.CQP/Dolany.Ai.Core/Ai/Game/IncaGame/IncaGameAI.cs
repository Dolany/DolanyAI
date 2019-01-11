namespace Dolany.Ai.Core.Ai.Game.IncaGame
{
    using System.Linq;

    using Base;
    using Cache;
    using Common;
    using Model;
    using Database.Incantation;

    using JetBrains.Annotations;

    [AI(
        Name = nameof(IncaGameAI),
        Description = "AI for Incantation game.",
        IsAvailable = false,
        PriorityLevel = 10)]
    public class IncaGameAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "新魔咒",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "创建一个新的魔咒",
            Syntax = "[魔咒名称] [魔咒咒文]",
            Tag = "游戏功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            IsGroupAvailable = false)]
        public void NewMagic(MsgInformationEx MsgDTO, object[] param)
        {
            var charactor = GetCharactor(MsgDTO.FromQQ);
            if (charactor == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "请先使用 创建人物 命令创建人物");
                return;
            }

            // todo
        }

        [CanBeNull]
        private static IncaCharactor GetCharactor(long QQNum)
        {
            using (var db = new IncaDatabase())
            {
                var c = db.IncaCharactor.FirstOrDefault(p => p.QQNum == QQNum);

                return c?.Clone();
            }
        }
    }
}
