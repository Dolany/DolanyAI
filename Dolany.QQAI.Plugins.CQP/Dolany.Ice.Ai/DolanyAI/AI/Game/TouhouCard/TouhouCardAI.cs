using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(TouhouCardAI),
        Description = "AI for Getting Random TouhouCard.",
        IsAvailable = true,
        PriorityLevel = 0
    )]
    public class TouhouCardAI : AIBase
    {
        public TouhouCardAI()
        {
            RuntimeLogger.Log("TouhouCardAI started");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "每日抽卡",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机获取一张DIY卡牌",
            Syntax = "",
            Tag = "游戏功能",
            SyntaxChecker = "Empty"
        )]
        public void RandomCard(GroupMsgDTO MsgDTO, object[] param)
        {
            using (new AIDatabase())
            {
            }
        }
    }
}