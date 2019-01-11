namespace Dolany.Ai.Core.API
{
    using System;

    using ViewModel;
    using Cache;
    using Common;
    using Net;
    using Dolany.Database.Ai;

    using JetBrains.Annotations;

    public class APIEx
    {
        [CanBeNull]
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

            return info?.Msg;
        }

        [CanBeNull]
        public static GroupMemberListViewModel GetMemberInfos(long GroupNum)
        {
            var ml = GetGroupMemberList(GroupNum.ToString());
            if (string.IsNullOrEmpty(ml))
            {
                return null;
            }

            try
            {
                return JsonHelper.DeserializeJsonToObject<GroupMemberListViewModel>(ml);
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                return null;
            }
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
