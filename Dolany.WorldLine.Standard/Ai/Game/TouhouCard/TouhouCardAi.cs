using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.Ai.Game.TouhouCard
{
    public class TouhouCardAi : AIBase
    {
        public override string AIName { get; set; } = "幻想乡抽卡";

        public override string Description { get; set; } = "AI for Getting Random TouhouCard.";

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.游戏功能;

        public TouhouCardSvc TouhouCardSvc { get; set; }

        [EnterCommand(ID = "TouhouCardAi_RandomCard",
            Command = ".card 幻想乡抽卡",
            Description = "随机获取一张DIY幻想乡卡牌(每日刷新)",
            IsPrivateAvailable = true)]
        public bool RandomCard(MsgInformationEx MsgDTO, object[] param)
        {
            var cardName = TouhouCardSvc.RandomCard(MsgDTO.FromQQ);
            TouhouCardSvc.ShowCard(cardName, MsgDTO);
            return true;
        }

        [EnterCommand(ID = "TouhouCardAi_ViewCard",
            Command = "查看卡牌",
            Description = "查看一张DIY幻想乡卡牌",
            IsPrivateAvailable = true)]
        public bool ViewCardCard(MsgInformationEx MsgDTO, object[] param)
        {
            var name = (string) param[0];
            var card = TouhouCardSvc[name];
            if (string.IsNullOrEmpty(card))
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关卡牌！");
                return false;
            }

            TouhouCardSvc.ShowCard(card, MsgDTO);
            return true;
        }
    }
}
