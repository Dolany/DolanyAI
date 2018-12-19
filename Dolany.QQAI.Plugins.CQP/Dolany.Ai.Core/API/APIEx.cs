namespace Dolany.Ai.Core.API
{
    using System;

    using Dolany.Ai.Core.API.ViewModel;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;
    using Dolany.Ai.Core.Net;

    public class APIEx
    {
        private static string GetGroupMemberList(string 群号)
        {
            var info = Waiter.Instance.WaitForRelationId(
                new MsgCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        Command = AiCommand.GetGroupMemberInfo,
                        Msg = 群号,
                        Time = DateTime.Now,
                        ToGroup = 0,
                        ToQQ = 0
                    });

            return info == null ? string.Empty : info.Msg;
        }

        public static GroupMemberListViewModel GetMemberInfos(long GroupNum)
        {
            var ml = GetGroupMemberList(GroupNum.ToString());
            return string.IsNullOrEmpty(ml) ? null : JsonHelper.DeserializeJsonToObject<GroupMemberListViewModel>(ml);
        }

        public static void SendPraise(long QQ号, int count = 10)
        {
            Waiter.Instance.WaitForRelationId(
                new MsgCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        Command = AiCommand.Praise,
                        Msg = count.ToString(),
                        Time = DateTime.Now,
                        ToQQ = QQ号
                    });
        }
    }
}
