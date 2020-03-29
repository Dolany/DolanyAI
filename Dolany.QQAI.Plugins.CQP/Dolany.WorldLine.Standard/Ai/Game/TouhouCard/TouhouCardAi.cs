using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.TouhouCard
{
    public class TouhouCardAi : AIBase
    {
        public override string AIName { get; set; } = "幻想乡抽卡";

        public override string Description { get; set; } = "AI for Getting Random TouhouCard.";

        public override CmdTagEnum DefaultTag { get; } = CmdTagEnum.游戏功能;

        private const string PicPath = "TouhouCard/";

        private List<FileInfo> AllFiles;

        public override void Initialization()
        {
            var dir = new DirectoryInfo(PicPath);
            AllFiles = dir.GetFiles().ToList();
        }

        [EnterCommand(ID = "TouhouCardAi_RandomCard",
            Command = ".card 幻想乡抽卡",
            Description = "随机获取一张DIY幻想乡卡牌(每日刷新)",
            IsPrivateAvailable = true)]
        public bool RandomCard(MsgInformationEx MsgDTO, object[] param)
        {
            var cardName = RandomCard(MsgDTO.FromQQ);
            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Image_Relational(cardName));
            return true;
        }

        private string RandomCard(long FromQQ)
        {
            var cache = PersonCacheRecord.Get(FromQQ, "TouhouCard");
            if (!string.IsNullOrEmpty(cache.Value))
            {
                return PicPath + cache.Value;
            }

            var card = GetRandCard();
            cache.Value = card;
            cache.ExpiryTime = CommonUtil.UntilTommorow();
            cache.Update();

            return PicPath + card;
        }

        private string GetRandCard()
        {
            var file = AllFiles.RandElement();
            return file.Name;
        }
    }
}
