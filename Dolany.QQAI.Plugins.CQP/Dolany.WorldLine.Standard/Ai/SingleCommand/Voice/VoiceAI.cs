using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.Voice
{
    public class VoiceAI : AIBase, IDataMgr
    {
        public override string AIName { get; set; } = "语音";

        public override string Description { get; set; } = "AI for response random acg voice.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        private Beng3ConfigModel Beng3Config;

        public void RefreshData()
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

        [EnterCommand(ID = "VoiceAI_LOLVoice",
            Command = "LOL语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取LOL的随机语音",
            Syntax = "",
            Tag = "语音功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool LOLVoice(MsgInformationEx MsgDTO, object[] param)
        {
            return RandVoice(MsgDTO, "LOL语音");
        }

        [EnterCommand(ID = "VoiceAI_超神学院Voice",
            Command = "超神学院语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取超神学院的随机语音",
            Syntax = "",
            Tag = "语音功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool 超神学院Voice(MsgInformationEx MsgDTO, object[] param)
        {
            return RandVoice(MsgDTO, "超神学院");
        }

        [EnterCommand(ID = "VoiceAI_刺客伍六七Voice",
            Command = "刺客伍六七语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取刺客伍六七的随机语音",
            Syntax = "",
            Tag = "语音功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool 刺客伍六七Voice(MsgInformationEx MsgDTO, object[] param)
        {
            return RandVoice(MsgDTO, "刺客伍六七");
        }

        [EnterCommand(ID = "VoiceAI_狐妖小红娘Voice",
            Command = "狐妖小红娘语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取狐妖小红娘的随机语音",
            Syntax = "",
            Tag = "语音功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool 狐妖小红娘Voice(MsgInformationEx MsgDTO, object[] param)
        {
            return RandVoice(MsgDTO, "狐妖小红娘");
        }

        [EnterCommand(ID = "VoiceAI_十万个冷笑话Voice",
            Command = "十万个冷笑话语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取十万个冷笑话的随机语音",
            Syntax = "",
            Tag = "语音功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool 十万个冷笑话Voice(MsgInformationEx MsgDTO, object[] param)
        {
            return RandVoice(MsgDTO, "十万个冷笑话");
        }

        [EnterCommand(ID = "VoiceAI_一人之下Voice",
            Command = "一人之下语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取一人之下的随机语音",
            Syntax = "",
            Tag = "语音功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool 一人之下Voice(MsgInformationEx MsgDTO, object[] param)
        {
            return RandVoice(MsgDTO, "一人之下");
        }

        [EnterCommand(ID = "VoiceAI_镇魂街Voice",
            Command = "镇魂街语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取镇魂街的随机语音",
            Syntax = "",
            Tag = "语音功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool 镇魂街Voice(MsgInformationEx MsgDTO, object[] param)
        {
            return RandVoice(MsgDTO, "镇魂街");
        }

        [EnterCommand(ID = "VoiceAI_秦时明月Voice",
            Command = "秦时明月语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取秦时明月的随机语音",
            Syntax = "",
            Tag = "语音功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool 秦时明月Voice(MsgInformationEx MsgDTO, object[] param)
        {
            return RandVoice(MsgDTO, "秦时明月");
        }

        private bool RandVoice(MsgInformationEx MsgDTO, string name)
        {
            var folderPath = "./voices/" + name;
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
