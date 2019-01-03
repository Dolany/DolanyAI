using System.Collections.Generic;

namespace Dolany.Ai.Core.Ai.SingleCommand.Dice
{
    using System.Diagnostics;
    using System.Linq;

    using Dolany.Ai.Core.API;
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

    [AI(
        Name = nameof(GrouperAI),
        Description = "AI for grouping some members.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class GrouperAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "分组",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "分组",
            Syntax = "",
            Tag = "骰子功能",
            SyntaxChecker = "Long MultiAt",
            IsPrivateAvailabe = false)]
        public void Group(MsgInformationEx MsgDTO, object[] param)
        {
            var size = (int)param[0];
            var list = param[1] as List<long>;

            Debug.Assert(list != null, nameof(list) + " != null");
            list = Utility.RandSort(list.ToArray()).ToList();

            for (var i = 0; i < list.Count; i += size)
            {
                var sublist = list.Skip(i).Take(4);
                var msg = $"第{i / size + 1}组：{string.Join(",", sublist.Select(CodeApi.Code_At))}";
                MsgSender.Instance.PushMsg(MsgDTO, msg);
            }
        }
    }
}
