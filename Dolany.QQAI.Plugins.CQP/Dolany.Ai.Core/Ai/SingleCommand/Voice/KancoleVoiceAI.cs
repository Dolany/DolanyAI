namespace Dolany.Ai.Core.Ai.SingleCommand.Voice
{
    using System.Linq;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Database;
    using Dolany.Database.Ai;

    using Model;

    using static API.CodeApi;

    [AI(
        Name = "舰娘语音",
        Description = "AI for response random kancole girl voice.",
        Enable = true,
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
            Tag = "娱乐功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 3)]
        public bool KancoleVoice(MsgInformationEx MsgDTO, object[] param)
        {
            var girlName = param[0] as string;

            var voice = GetRandVoiceInfo(girlName);
            if (voice == null)
            {
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, Code_Voice(voice.VoiceUrl));
            MsgSender.Instance.PushMsg(MsgDTO, voice.Content);
            return true;
        }

        private static KanColeGirlVoice GetRandVoiceInfo(string name)
        {
            var query = MongoService<KanColeGirlVoice>.Get(p => p.Name.Contains(name))
                .OrderBy(p => p.Id).ToList();
            if (query.IsNullOrEmpty())
            {
                return null;
            }

            var count = query.Count;
            var idx = CommonUtil.RandInt(count);

            return query.Skip(idx).First().Clone();
        }
    }
}
