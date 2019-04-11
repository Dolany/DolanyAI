using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.SingleCommand.OrderSong
{
    using Base;

    using Cache;
    using Model;

    using Net;

    [AI(
        Name = "点歌",
        Description = "AI for Ordering a song by name.",
        Enable = true,
        PriorityLevel = 10)]
    public class OrderSongAI : AIBase
    {
        public override void Initialization()
        {
        }

        [EnterCommand(
            Command = "点歌",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "根据歌名点歌",
            Syntax = "[歌名]",
            Tag = "娱乐功能",
            SyntaxChecker = "Any",
            IsPrivateAvailable = true,
            DailyLimit = 3,
            TestingDailyLimit = 3)]
        public bool OrderASong(MsgInformationEx MsgDTO, object[] param)
        {
            var songName = param[0] as string;
            var songId = GetSongId(songName);

            if (!string.IsNullOrEmpty(songId))
            {
                var responseXml = GetMusicXml(songId);
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
            return song?.Msg;
        }
    }
}
