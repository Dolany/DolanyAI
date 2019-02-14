using System.Linq;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database.Sqlite;
using Dolany.Game.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    [AI(
        Name = "签到",
        Description = "AI for Everyday Signing In.",
        Enable = false,
        PriorityLevel = 1,
        NeedManulOpen = true)]
    public class SignInAI : AIBase
    {
        [EnterCommand(Command = "签到",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置今日签到内容(不能与系统自带命令重复)",
            Syntax = "[签到内容]",
            Tag = "商店功能",
            SyntaxChecker = "Any",
            IsPrivateAvailable = false)]
        public void SetSignContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            if (AIMgr.Instance.AllAvailableGroupCommands.Any(comm => comm.Command == content))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "不能与系统自带命令重复！");
                return;
            }

            SCacheService.Cache($"DailySignIn-{MsgDTO.FromGroup}", content);
            MsgSender.Instance.PushMsg(MsgDTO, "设置成功！");
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            var signCache = SCacheService.Get<string>($"DailySignIn-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}");
            if (!string.IsNullOrEmpty(signCache))
            {
                return false;
            }

            var cache = SCacheService.Get<string>($"DailySignIn-{MsgDTO.FromGroup}");
            if (string.IsNullOrEmpty(cache) && MsgDTO.FullMsg != "签到")
            {
                return false;
            }

            Sign(MsgDTO);
            return true;
        }

        private void Sign(MsgInformationEx MsgDTO)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            // todo
        }
    }
}
