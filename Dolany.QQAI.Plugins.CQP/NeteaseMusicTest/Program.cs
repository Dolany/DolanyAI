using Dolany.Ai.Reborn.DolanyAI.Model;
using Dolany.Ai.Reborn.DolanyAI.Net;
using static Dolany.Ai.Reborn.DolanyAI.Common.Utility;

namespace NeteaseMusicTest
{
    internal class Program
    {
        private static void Main()
        {
            var songName = "安娜";
            RequestHelper.PostData<NeteaseResponse>(new PostReq_Param
            {
                InterfaceName = $"http://music.163.com/api/search/get/?s={UrlCharConvert(songName)}&type=1"
            });
        }
    }
}