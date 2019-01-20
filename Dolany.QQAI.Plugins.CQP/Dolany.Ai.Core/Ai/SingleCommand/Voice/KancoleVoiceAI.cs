namespace Dolany.Ai.Core.Ai.SingleCommand.Voice
{
    using System.Linq;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Dolany.Database;
    using Dolany.Database.Ai;

    using Model;

    using static Dolany.Ai.Core.API.CodeApi;

    [AI(
        Name = nameof(KancoleVoiceAI),
        Description = "AI for response random kancole girl voice.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class KancoleVoiceAI : AIBase
    {
        public override void Initialization()
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
            var query = MongoService<KanColeGirlVoice>.Get(p => p.Name.Contains(name))
                .OrderBy(p => p.Id).ToList();
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
