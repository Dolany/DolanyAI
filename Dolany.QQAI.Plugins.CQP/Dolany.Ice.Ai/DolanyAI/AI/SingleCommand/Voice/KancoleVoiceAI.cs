﻿using System;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(KancoleVoiceAI),
        Description = "AI for response random kancole girl voice.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class KancoleVoiceAI : AIBase
    {
        public KancoleVoiceAI()
        {
            RuntimeLogger.Log("KancoleVoiceAI started.");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "舰娘语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取随机舰娘语音",
            Syntax = "[舰娘名称]",
            Tag = "语音功能",
            SyntaxChecker = "NotEmpty",
            IsDeveloperOnly = false,
            IsPrivateAvailabe = true
            )]
        public void KancoleVoice(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var girlName = param[0] as string;

            var voice = GetRandVoiceInfo(girlName);
            if (voice == null)
            {
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, CodeApi.Code_Voice(voice.VoiceUrl));
            MsgSender.Instance.PushMsg(MsgDTO, voice.Content);
        }

        private static KanColeGirlVoice GetRandVoiceInfo(string name)
        {
            using (var db = new AIDatabase())
            {
                var query = db.KanColeGirlVoice.Where(p => p.Name.Contains(name)).OrderBy(p => p.Id);
                if (query.IsNullOrEmpty())
                {
                    return null;
                }

                var count = query.Count();
                var idx = Utility.RandInt(count);

                return query.Skip(idx).First().Clone();
            }
        }
    }
}