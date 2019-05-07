﻿using Dolany.Ai.Common;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.API
{
    using System;

    using ViewModel;
    using Cache;
    using Common;
    using Net;
    using JetBrains.Annotations;

    public class APIEx
    {
        [CanBeNull]
        private static string GetGroupMemberList(string 群号)
        {
            var info = Waiter.Instance.WaitForRelationId(
                new MsgCommand
                    {
                        Command = AiCommand.GetGroupMemberInfo,
                        Msg = 群号,
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
                Logger.Log(ml);
                return JsonConvert.DeserializeObject<GroupMemberListViewModel>(ml);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static void SendPraise(long QQ号, int count = 10)
        {
            Waiter.Instance.WaitForRelationId(
                new MsgCommand
                    {
                        Command = AiCommand.Praise,
                        Msg = count.ToString(),
                        ToQQ = QQ号
                    });
        }
    }
}
