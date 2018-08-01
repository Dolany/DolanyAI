using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = "OrderSongAI",
        Description = "AI for Ordering a song by name.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class OrderSongAI : AIBase
    {
        public OrderSongAI()
            : base()
        {
            RuntimeLogger.Log("OrderSongAI started.");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "点歌",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "根据歌名点歌",
            Syntax = "[歌名]",
            Tag = "点歌功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void OrderASong(GroupMsgDTO MsgDTO, object[] param)
        {
            string songName = param[0] as string;

            string songId = GetSongId(songName);

            SendMusic(songId);
        }

        private string GetSongId(string songName)
        {
            NetEaseMusicParser parser = new NetEaseMusicParser();
            HttpRequester requester = new HttpRequester();
            string aimStr = $"https://music.163.com/#/search/m/?s={Utility.UrlCharConvert(songName)}&type=1";
            string HtmlStr = requester.Request(aimStr);

            parser.Load(HtmlStr);

            return parser.SongId;
        }

        private void SendMusic(string songId)
        {
            if (string.IsNullOrEmpty(songId))
            {
                return;
            }

            // TODO
        }
    }
}