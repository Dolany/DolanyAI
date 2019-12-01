using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.Net;

namespace Dolany.Ai.Core.Ai.SingleCommand.OrderSong
{
    public class OrderSongAI : AIBase
    {
        public override string AIName { get; set; } = "点歌";

        public override string Description { get; set; } = "AI for Ordering a song by name.";

        public override int PriorityLevel { get; set; } = 10;

        public override bool IsAdvanced { get; set; } = true;

        public override bool Enable { get; set; } = false;

        [EnterCommand(ID = "OrderSongAI_OrderASong",
            Command = "点歌",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "根据歌名点歌",
            Syntax = "[歌名]",
            Tag = "娱乐功能",
            SyntaxChecker = "Any",
            IsPrivateAvailable = true,
            DailyLimit = 5,
            TestingDailyLimit = 5)]
        public bool OrderASong(MsgInformationEx MsgDTO, object[] param)
        {
            var songName = param[0] as string;
            var songId = GetSongId(songName);

            if (!string.IsNullOrEmpty(songId))
            {
                var responseXml = GetMusicXml(songId, MsgDTO.BindAi);
                MsgSender.PushMsg(MsgDTO, responseXml);

                return true;
            }

            MsgSender.PushMsg(MsgDTO, "未查找到该歌曲！");
            return false;
        }

        private static string GetSongId(string songName)
        {
            var response = RequestHelper.PostData<NeteaseResponseModel>(new PostReq_Param
            {
                InterfaceName = $"http://music.163.com/api/search/get/?s={System.Web.HttpUtility.UrlEncode(songName)}&type=1"
            });

            if (response?.Result == null)
            {
                return string.Empty;
            }

            var songs = response.Result.Songs;
            if (songs.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var song = songs.RandElement();
            return song.Id;
        }

        private static string GetMusicXml(string songId, string BindAi)
        {
            if (string.IsNullOrEmpty(songId))
            {
                return string.Empty;
            }

            var song = Waiter.Instance.WaitForRelationId(
                new MsgCommand
                    {
                        Command = CommandType.Get163Music,
                        Msg = songId,
                        BindAi = BindAi
                    });
            return song?.Msg;
        }
    }
}
