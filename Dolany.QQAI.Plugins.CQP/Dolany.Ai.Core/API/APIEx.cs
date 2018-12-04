using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.API
{
    using Dolany.Ai.Core.API.ViewModel;
    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Net;

    public class APIEx
    {
        private static string GetGroupMemberList(string 群号)
        {
            try
            {
                var AuthCode = Utility.GetAuthCode();
                return Api_GetGroupMemberList(群号, AuthCode);
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                return string.Empty;
            }
        }

        public static GroupMemberListViewModel GetMemberInfos(long GroupNum)
        {
            var ml = GetGroupMemberList(GroupNum.ToString());
            return string.IsNullOrEmpty(ml) ? null : JsonHelper.DeserializeJsonToObject<GroupMemberListViewModel>(ml);
        }
    }
}
