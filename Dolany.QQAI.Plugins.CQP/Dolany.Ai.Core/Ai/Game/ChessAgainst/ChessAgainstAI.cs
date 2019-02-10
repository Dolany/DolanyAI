using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Game.ChessAgainst
{
    [AI(
        Name = nameof(ChessAgainstAI),
        Description = "AI for Chess Fight.",
        Enable = false,
        PriorityLevel = 10)]
    public class ChessAgainstAI : AIBase
    {
        [EnterCommand(Command = "对决",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "指定一名成员进行对决",
            Syntax = "[@QQ号]",
            Tag = "游戏功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public void Sell(MsgInformationEx MsgDTO, object[] param)
        {

        }
    }
}
