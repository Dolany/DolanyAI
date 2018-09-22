using System.Linq;
using System;
using Dolany.Ice.Ai.DolanyAI.Utils;
using static Dolany.Ice.Ai.DolanyAI.Utils.Utility;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(OrderSongAI),
        Description = "AI for Ordering a song by name.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
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
            SyntaxChecker = "Word",
            IsPrivateAvailabe = true
            )]
        public void OrderASong(ReceivedMsgDTO MsgDTO, object[] param)
        {
            try
            {
                var songName = param[0] as string;

                var songId = GetSongId(songName);

                var responseXml = GetMusicXml(songId);
                MsgSender.Instance.PushMsg(MsgDTO, responseXml);
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
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
            var idx = Utility.RandInt(songs.Count());
            return response.result.songs.ElementAt(idx).id;
        }

        private static string GetMusicXml(string songId)
        {
            return string.IsNullOrEmpty(songId) ? string.Empty : AmandaAPIEx._163Music(songId);
        }
    }
}