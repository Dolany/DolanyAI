using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = "KancoleVoiceAI",
        Description = "AI for response random kancole girl voice.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class KancoleVoiceAI : AIBase
    {
        public KancoleVoiceAI()
            : base()
        {
            RuntimeLogger.Log("KancoleVoiceAI started.");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "舰娘语音",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取随机舰娘语音",
            Syntax = "[舰娘名称]",
            Tag = "语音功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void KancoleVoice(GroupMsgDTO MsgDTO, object[] param)
        {
            string girlName = param[0] as string;

            var voice = GetRandVoiceInfo(girlName);
            if (voice == null)
            {
                return;
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = CodeApi.Code_Voice(voice.VoiceUrl)
            });
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = voice.Content
            });
        }

        private KanColeGirlVoice GetRandVoiceInfo(string name)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.KanColeGirlVoice.Where(p => p.Name.Contains(name)).OrderBy(p => p.Id);
                if (query.IsNullOrEmpty())
                {
                    return null;
                }

                int count = query.Count();
                Random random = new Random();
                int idx = random.Next(count);

                return query.Skip(idx).First().Clone();
            }
        }
    }
}