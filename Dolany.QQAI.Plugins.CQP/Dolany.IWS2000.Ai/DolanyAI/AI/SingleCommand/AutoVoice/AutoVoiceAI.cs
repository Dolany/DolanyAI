using System;
using System.Linq;
using Dolany.IWS2000.Ai.DolanyAI.Db;
using Dolany.IWS2000.Ai.MahuaApis;

namespace Dolany.IWS2000.Ai.DolanyAI
{
    [AI(
        Name = nameof(AutoVoiceAI),
        Description = "AI for auto repeating voice/msg.",
        IsAvailable = true,
        PriorityLevel = 10
    )]
    public class AutoVoiceAI : AIBase
    {
        public AutoVoiceAI()
        {
            RuntimeLogger.Log("AutoVoiceAI started.");
        }

        public override void Work()
        {
            using (var db = new AIDatabase())
            {
                var query = db.VoicesData.Select(p => p.Tag).Distinct();
                foreach (var voiceTag in query)
                {
                    Consolers.Add(new GroupEnterCommandAttribute
                    {
                        Command = voiceTag,
                        SyntaxChecker = "Empty",
                        AuthorityLevel = AuthorityLevel.成员
                    }, VoiceCommand);
                }
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void VoiceCommand(GroupMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.VoicesData.Where(p => p.Tag == MsgDTO.Command)
                                         .OrderBy(p => p.Id);
                if (query.IsNullOrEmpty())
                {
                    return;
                }

                var rand = new Random();
                var idx = rand.Next(query.Count());

                var voice = query.Skip(idx).FirstOrDefault();
                SendVoice(MsgDTO, voice);
            }
        }

        [GroupEnterCommand(
            Command = "随机语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取一条随机语音",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "语音功能"
        )]
        public void RandVoice(GroupMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var query = db.VoicesData.OrderBy(p => p.Id);

                var rand = new Random();
                var idx = rand.Next(query.Count());

                var voice = query.Skip(idx).FirstOrDefault();
                SendVoice(MsgDTO, voice);
            }
        }

        private static void SendVoice(GroupMsgDTO MsgDTO, VoicesData voice)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Msg = voice.Content,
                Type = MsgType.Group
            }, new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Msg = CodeApi.Code_Voice(voice.VoiceUrl),
                Type = MsgType.Group
            });
        }
    }
}