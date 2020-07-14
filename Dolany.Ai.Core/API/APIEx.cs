using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.API.ViewModel;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.API
{
    public class APIEx
    {
        private static WaiterSvc WaiterSvc => AutofacSvc.Resolve<WaiterSvc>();

        [CanBeNull]
        private static string GetGroupMemberList(string 群号, string BindAi)
        {
            var info = WaiterSvc.WaitForRelationId(
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
            WaiterSvc.WaitForRelationId(
                new MsgCommand
                {
                    Command = CommandType.Praise,
                    Msg = count.ToString(),
                    ToQQ = QQ号,
                    BindAi = BindAi
                });
        }

        public static QQInfoModel GetQQInfo(long QQNum, string BindAi)
        {
            var info = WaiterSvc.WaitForRelationId(new MsgCommand {Command = CommandType.GetQQInfo, ToQQ = QQNum, BindAi = BindAi});
            //RuntimeLogger.Log(JsonConvert.SerializeObject(info));
            return info == null ? null : JsonConvert.DeserializeObject<QQInfoModel>(info.Msg);
        }
    }
}
