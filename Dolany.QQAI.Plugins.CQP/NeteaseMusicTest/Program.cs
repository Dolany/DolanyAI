﻿using Dolany.Ice.Ai.DolanyAI;

namespace NeteaseMusicTest
{
    internal class Program
    {
        private static void Main()
        {
            var songName = "安娜";
            RequestHelper.PostData<NeteaseResponse>(new PostReq_Param
            {
                InterfaceName = $"http://music.163.com/api/search/get/?s={Utility.UrlCharConvert(songName)}&type=1"
            });
        }
    }
}