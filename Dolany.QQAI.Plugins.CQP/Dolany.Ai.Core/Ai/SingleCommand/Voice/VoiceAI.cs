using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.SingleCommand.Voice
{
    [AI(Name = "语音",
        Description = "AI for response random acg voice.",
        Enable = true,
        PriorityLevel = 10)]
    public class VoiceAI : AIBase
    {
        private Beng3ConfigModel Beng3Config;

        public override void Initialization()
        {
            Beng3Config = CommonUtil.ReadJsonData<Beng3ConfigModel>("Beng3VoiceConfigData");
        }

        [EnterCommand(ID = "VoiceAI_KancoleVoice",
            Command = "舰娘语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取随机舰娘语音",
            Syntax = "[舰娘名称]",
            Tag = "语音功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
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

        [EnterCommand(ID = "VoiceAI_Beng3Voice",
            Command = "崩三语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取指定崩三女武神的随机语音",
            Syntax = "[女武神名称]",
            Tag = "语音功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool Beng3Voice(MsgInformationEx MsgDTO, object[] param)
        {
            var girlName = param[0] as string;

            var voiceSet = Beng3Config.Config.FirstOrDefault(p => p.Names.Contains(girlName));
            if (voiceSet == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关信息！", true);
                return false;
            }

            var folderPath = Beng3Config.VoicePath + voiceSet.Path;
            var folder = new DirectoryInfo(folderPath);
            var files = folder.GetFiles();
            var file = files.RandElement();

            MsgSender.PushMsg(MsgDTO, CodeApi.Code_Voice(file.FullName));
            return true;
        }
    }

    public class Beng3ConfigModel
    {
        public string VoicePath { get; set; }

        public Beng3VoiceSetModel[] Config { get; set; }
    }

    public class Beng3VoiceSetModel
    {
        public string[] Names { get; set; }

        public string Path { get; set; }
    }
}
