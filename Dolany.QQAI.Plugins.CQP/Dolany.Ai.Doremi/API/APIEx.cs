﻿using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.API.ViewModel;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Dolany.Ai.Doremi.API
{
    public class APIEx
    {
        [CanBeNull]
        private static string GetGroupMemberList(string 群号, string BindAi)
        {
            var info = Waiter.Instance.WaitForRelationId(
                new MsgCommand
                {
                    Command = CommandType.GetGroupMemberInfo,
                    Msg = 群号,
                    ToGroup = 0,
                    ToQQ = 0,
                    BindAi = BindAi
                });

            return info?.Msg;
        }

        [CanBeNull]
        public static GroupMemberListViewModel GetMemberInfos(long GroupNum, string BindAi)
        {
            var ml = GetGroupMemberList(GroupNum.ToString(), BindAi);
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

        public static void SendPraise(long QQ号, string BindAi, int count = 10)
        {
            Waiter.Instance.WaitForRelationId(
                new MsgCommand
                {
                    Command = CommandType.Praise,
                    Msg = count.ToString(),
                    ToQQ = QQ号,
                    BindAi = BindAi
                });
        }
    }
}