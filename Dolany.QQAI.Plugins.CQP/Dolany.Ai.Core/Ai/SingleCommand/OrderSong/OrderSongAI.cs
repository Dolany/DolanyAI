using Dolany.Database.Sqlite;
using Dolany.Database.Sqlite.Model;

namespace Dolany.Ai.Core.Ai.SingleCommand.OrderSong
{
    using System.Linq;

    using Base;

    using Cache;

    using Common;

    using Dolany.Database.Ai;

    using Model;

    using Net;

    using static Common.Utility;

    [AI(
        Name = nameof(OrderSongAI),
        Description = "AI for Ordering a song by name.",
        Enable = true,
        PriorityLevel = 10)]
    public class OrderSongAI : AIBase
    {
        private const int DailyLimit = 3;

        public override void Initialization()
        {
        }

        [EnterCommand(
            Command = "点歌",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "根据歌名点歌",
            Syntax = "[歌名]",
            Tag = "点歌功能",
            SyntaxChecker = "Any",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public void OrderASong(MsgInformationEx MsgDTO, object[] param)
        {
            var songName = param[0] as string;
            var songId = GetSongId(songName);

            if (!string.IsNullOrEmpty(songId))
            {
                var responseXml = GetMusicXml(songId);
                MsgSender.Instance.PushMsg(MsgDTO, responseXml);

                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, "未查找到该歌曲！");
        }

        private static string GetSongId(string songName)
        {
            var response = RequestHelper.PostData<NeteaseResponseModel>(new PostReq_Param
            {
                InterfaceName = $"http://music.163.com/api/search/get/?s={UrlCharConvert(songName)}&type=1"
            });

            if (response?.Result == null)
            {
                return string.Empty;
            }

            var songs = response.Result.Songs.ToList();
            var idx = RandInt(songs.Count);
            return songs[idx].Id;
        }

        private static string GetMusicXml(string songId)
        {
            if (string.IsNullOrEmpty(songId))
            {
                return string.Empty;
            }

            var song = Waiter.Instance.WaitForRelationId(
                new MsgCommand
                    {
                        Command = AiCommand.Get163Music,
                        Msg = songId
                    });
            return song.Msg;
        }
    }
}
