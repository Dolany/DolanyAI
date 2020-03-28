using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.WorldLine.KindomStorm.Ai.Welcome
{
    public class WelcomeAI : AIBase
    {
        public override string AIName { get; set; } = "入群欢迎";
        public override string Description { get; set; } = "Ai for Welcome.";
        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        public override bool OnGroupMemberChanged(GroupMemberChangeModel model)
        {
            var group = GroupSettingSvc[model.GroupNum];
            if (model.Type == 1)
            {
                group.MembersCount--;
                group.Update();
                return true;
            }

            group.MembersCount++;
            group.Update();

            var msg = $"欢迎加入本群！你是本群第{group.MembersCount}名成员！\r\n";
            msg += "----------\r\n";
            msg += "友情提示：加入任何群都要先看群公告哦！\r\n";
            msg += "请热爱本群，勿轻易退出\r\n";
            msg += "请遵守群规，勿漠视管理\r\n";
            msg += "请活跃气氛，勿长期潜水\r\n";
            msg += "请谨言慎行，勿广告刷屏\r\n";
            msg += "请团结友爱，勿攻击谩骂\r\n";
            msg += "请公开讨论，勿私聊骚扰\r\n";

            MsgSender.PushMsg(model.GroupNum, model.QQNum, msg, group.BindAi);
            return true;
        }
    }
}
