namespace Dolany.Ai.Core.Ai.Game.TouhouCardWar
{
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Model;

    [AI(
        Name = nameof(TouhouCardWarAi),
        Description = "AI for Touhou Card War Game.",
        IsAvailable = false,
        PriorityLevel = 10)]
    public class TouhouCardWarAi : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "GameStart",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "Start Touhou Card War!",
            Syntax = "",
            Tag = "游戏功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public void GameStart(MsgInformationEx MsgDTO, object[] param)
        {
        }

        private bool CreateCharactor(MsgInformationEx MsgDTO)
        {
            // todo
            return false;
        }
    }
}
