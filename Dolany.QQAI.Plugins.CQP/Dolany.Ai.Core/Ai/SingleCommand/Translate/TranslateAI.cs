namespace Dolany.Ai.Core.Ai.SingleCommand.Translate
{
    using System.Linq;

    using Base;
    using Cache;

    using Dolany.Ai.Common;

    using Model;
    using Net;

    [AI(Name = nameof(TranslateAI),
        Description = "Ai for translation",
        IsAvailable = false,
        PriorityLevel = 10)]
    public class TranslateAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "翻译",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "翻译指定内容",
            Syntax = "[翻译内容]",
            Tag = "翻译功能",
            SyntaxChecker = "Any",
            IsPrivateAvailable = false)]
        public void Trans(MsgInformationEx MsgDTO, object[] param)
        {
            var word = param[0] as string;
            var res = RequestHelper.PostData<TranslationReceiveModel>(
                new PostReq_Param
                    {
                        data = new TranslationSendModel { Word = word },
                        InterfaceName = Configger.Instance["TranslationAPIUrl"]
                    });

            if (res == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "翻译异常！");
                return;
            }

            var msg = string.Empty;
            switch (res.AimLang)
            {
                case "en":
                    var enwords = res.Meanings.Select(p => $"{p.Property} {string.Join(";", p.Details)}");
                    msg = string.Join("\r", enwords);
                    break;
                case "zh":
                    var zhwords = res.Meanings.SelectMany(m => m.Details);
                    msg = string.Join(";", zhwords);
                    break;
            }

            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }
    }
}
