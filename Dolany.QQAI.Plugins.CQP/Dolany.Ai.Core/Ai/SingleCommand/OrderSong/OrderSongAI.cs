using System;
using System.Linq;

namespace Dolany.Ai.Core.Ai.SingleCommand.OrderSong
{
    using Base;
    using Cache;
    using Common;
    using Model;
    using Net;
    using Dolany.Database.Ai;

    using static Common.Utility;

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
            IsPrivateAvailable = true)]
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
                        Id = Guid.NewGuid().ToString(),
                        Command = AiCommand.Get163Music,
                        Msg = songId,
                        Time = DateTime.Now
                    });
            return song.Msg;
        }
    }
}
