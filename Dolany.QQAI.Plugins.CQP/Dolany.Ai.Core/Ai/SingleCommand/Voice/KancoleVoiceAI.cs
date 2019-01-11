using System.Linq;

namespace Dolany.Ai.Core.Ai.SingleCommand.Voice
{
    using Base;
    using Cache;
    using Common;
    using Model;
    using Dolany.Database.Ai;

    using static API.CodeApi;

    [AI(
        Name = nameof(KancoleVoiceAI),
        Description = "AI for response random kancole girl voice.",
        IsAvailable = true,
        PriorityLevel = 10)]
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
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public void KancoleVoice(MsgInformationEx MsgDTO, object[] param)
        {
            var girlName = param[0] as string;

            var voice = GetRandVoiceInfo(girlName);
            if (voice == null)
            {
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, Code_Voice(voice.VoiceUrl));
            MsgSender.Instance.PushMsg(MsgDTO, voice.Content);
        }

        private static KanColeGirlVoice GetRandVoiceInfo(string name)
        {
            using (var db = new AIDatabase())
            {
                var query = db.KanColeGirlVoice.Where(p => p.Name.Contains(name))
                    .OrderBy(p => p.Id);
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
