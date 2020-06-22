using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.Ai.Game.TouhouCard
{
    public class TouhouCardSvc : IDependency
    {
        private List<FileInfo> AllFiles { get; }

        private const string PicPath = "TouhouCard/";

        public string this[string cardName] => AllFiles.FirstOrDefault(file => file.Name.Contains(cardName))?.Name;

        public TouhouCardSvc()
        {
            var dir = new DirectoryInfo(PicPath);
            AllFiles = dir.GetFiles().ToList();
        }

        public static void ShowCard(string cardName, MsgInformationEx MsgDTO)
        {
            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Image_Relational(PicPath + cardName));
        }

        public string RandomCard(long FromQQ)
        {
            return RapidCacher.GetCache($"TouhouCard:{FromQQ}", CommonUtil.UntilTommorow(), GetRandCard);
        }

        public string GetRandCard()
        {
            var file = AllFiles.RandElement();
            return file.Name;
        }
    }
}
