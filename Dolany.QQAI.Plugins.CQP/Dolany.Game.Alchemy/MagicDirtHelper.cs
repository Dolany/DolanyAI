using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Game.Alchemy
{
    public class MagicDirtHelper
    {
        public static MagicDirtHelper Instance { get; } = new MagicDirtHelper();

        private readonly List<string> MagicDirtList = new List<string>()
        {
            "赤影粉", "紫玉粉", "银石粉", "金火粉", "黑魔粉"
        };

        private MagicDirtHelper()
        {

        }

        public string RandomDirt()
        {
            return MagicDirtList[CommonUtil.RandInt(MagicDirtList.Count)];
        }
    }
}
