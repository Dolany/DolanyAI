using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.SingleCommand.Voice
{
    [AI(Name = "崩三语音",
        Description = "AI for response random beng3 girl voice.",
        Enable = true,
        PriorityLevel = 10)]
    public class Beng3VoiceAI : AIBase
    {
        private Beng3ConfigModel Config;

        public override void Initialization()
        {
            Config = CommonUtil.ReadJsonData<Beng3ConfigModel>("Beng3VoiceConfigData");
        }

        [EnterCommand(ID = "Beng3VoiceAI_Beng3Voice",
            Command = "崩三语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取指定崩三女武神的随机语音",
            Syntax = "[女武神名称]",
            Tag = "娱乐功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 5)]
        public bool Beng3Voice(MsgInformationEx MsgDTO, object[] param)
        {
            var girlName = param[0] as string;

            var voiceSet = Config.Config.FirstOrDefault(p => p.Names.Contains(girlName));
            if (voiceSet == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到相关信息！", true);
                return false;
            }

            var folderPath = Config.VoicePath + voiceSet.Path;
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
