using Dolany.Ice.Ai.DolanyAI.Utils;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(QRGenAI),
        Description = "AI for Generating a QR.",
        IsAvailable = true,
        PriorityLevel = 10
    )]
    public class QRGenAI : AIBase
    {
        public QRGenAI()
        {
            RuntimeLogger.Log("QRGenAI started.");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "二维码",
            Description = "生成二维码",
            Syntax = "[内容]",
            Tag = "二维码功能",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailabe = true
        )]
        public void GenQR(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var content = param[0] as string;
            var QRUrl = $@"http://qr.liantu.com/api.php?bg=f3f3f3&fg=ff0000&gc=222222&el=l&w=200&m=10&text={content}";
            MsgSender.Instance.PushMsg(MsgDTO, CodeApi.Code_Image(QRUrl), true);
        }
    }
}