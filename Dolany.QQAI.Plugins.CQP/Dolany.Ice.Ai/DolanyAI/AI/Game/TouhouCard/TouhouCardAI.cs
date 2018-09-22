using System;
using System.IO;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Utils;
using static Dolany.Ice.Ai.DolanyAI.Utils.CodeApi;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(TouhouCardAi),
        Description = "AI for Getting Random TouhouCard.",
        IsAvailable = true,
        PriorityLevel = 10
    )]
    public class TouhouCardAi : AIBase
    {
        public TouhouCardAi()
        {
            RuntimeLogger.Log("TouhouCardAI started");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = ".card 幻想乡抽卡",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "随机获取一张DIY幻想乡卡牌",
            Syntax = "",
            Tag = "抽卡功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = true
        )]
        public void RandomCard(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var cardName = TouhouCardRecordBLL.RandomCard(MsgDTO.FromQQ);
            MsgSender.Instance.PushMsg(MsgDTO, Code_Image(new FileInfo(cardName).FullName));
        }
    }
}