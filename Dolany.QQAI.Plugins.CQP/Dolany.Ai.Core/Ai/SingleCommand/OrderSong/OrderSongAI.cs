using System;
using System.Linq;

namespace Dolany.Ai.Core.Ai.SingleCommand.OrderSong
{
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;
    using Dolany.Ai.Core.Model;
    using Dolany.Ai.Core.Net;
    using static Dolany.Ai.Core.Common.Utility;

    [AI(
        Name = nameof(OrderSongAI),
        Description = "AI for Ordering a song by name.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class OrderSongAI : AIBase
    {
        public OrderSongAI()
        {
            RuntimeLogger.Log("OrderSongAI started.");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "点歌",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "根据歌名点歌",
            Syntax = "[歌名]",
            Tag = "点歌功能",
            SyntaxChecker = "Any",
            IsPrivateAvailabe = true)]
        public void OrderASong(MsgInformationEx MsgDTO, object[] param)
        {
            var songName = param[0] as string;

            var songId = GetSongId(songName);

            if (!string.IsNullOrEmpty(songId))
            {
                var responseXml = GetMusicXml(songId);
                MsgSender.Instance.PushMsg(MsgDTO, responseXml);
            }
            else
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未查找到该歌曲！");
            }
        }

        private static string GetSongId(string songName)
        {
            var response = RequestHelper.PostData<NeteaseResponseModel>(new PostReq_Param
            {
                InterfaceName = $"http://music.163.com/api/search/get/?s={UrlCharConvert(songName)}&type=1"
            });

            if (response == null)
            {
                return string.Empty;
            }

            var songs = response.result.songs;
            var idx = RandInt(songs.Count());
            return response.result.songs.ElementAt(idx).id;
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
                        Id = Guid.NewGuid().ToString(),
                        Command = AiCommand.Get163Music,
                        Msg = songId,
                        Time = DateTime.Now
                    });
            return song.Msg;
        }
    }
}
