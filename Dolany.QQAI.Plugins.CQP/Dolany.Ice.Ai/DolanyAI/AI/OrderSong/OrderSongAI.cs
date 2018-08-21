using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Dolany.Ice.Ai.MahuaApis;

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
            var songName = param[0] as string;

            var songId = GetSongId(songName);

            var responseXml = GetMusicXml(songId);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = responseXml
            });
        }

        private static string GetSongId(string songName)
        {
            var response = RequestHelper.PostData<NeteaseResponseModel>(new PostReq_Param
            {
                InterfaceName = $"http://music.163.com/api/search/get/?s={Utility.UrlCharConvert(songName)}&type=1"
            });

            if (response == null)
            {
                return string.Empty;
            }

            return response.result.songs.First().id;
        }

        private static string GetMusicXml(string songId)
        {
            if (string.IsNullOrEmpty(songId))
            {
                return string.Empty;
            }

            return AmandaAPIEx._163Music(songId);
        }
    }
}