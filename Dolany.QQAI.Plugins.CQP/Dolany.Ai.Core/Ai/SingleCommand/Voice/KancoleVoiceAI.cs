using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.SingleCommand.Voice
{
    [AI(Name = "舰娘语音",
        Description = "AI for response random kancole girl voice.",
        Enable = true,
        PriorityLevel = 10)]
    public class KancoleVoiceAI : AIBase
    {
        [EnterCommand(ID = "KancoleVoiceAI_KancoleVoice",
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

            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Voice(voice.VoiceUrl));
            MsgSender.PushMsg(MsgDTO, voice.Content);
            return true;
        }

        private static KanColeGirlVoice GetRandVoiceInfo(string name)
        {
            var query = MongoService<KanColeGirlVoice>.Get(p => p.Name.Contains(name));
            return query.IsNullOrEmpty() ? null : query.RandElement();
        }
    }
}
