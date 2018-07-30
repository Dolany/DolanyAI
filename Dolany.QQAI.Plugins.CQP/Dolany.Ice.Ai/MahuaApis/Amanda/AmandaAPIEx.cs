﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Dolany.Ice.Ai.DolanyAI;

namespace Dolany.Ice.Ai.MahuaApis
{
    public class AmandaAPIEx
    {
        //public static string AuthCode = "2160";

        [DllImport("bin\\message.dll")]
        private static extern string Api_GetGroupMemberList(string 群号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SendPraise(string QQ号, string AuthCode);

        private static string GetGroupMemberList(string 群号)
        {
            string AuthCode = Utility.GetAuthCode();
            return Api_GetGroupMemberList(群号, AuthCode);
        }

        public static GroupMemberListViewModel GetMemberInfos(long GroupNum)
        {
            return JsonHelper.DeserializeJsonToObject<GroupMemberListViewModel>(GetGroupMemberList(GroupNum.ToString()));
        }

        public static int SendPraise(string QQ号)
        {
            string AuthCode = Utility.GetAuthCode();
            return Api_SendPraise(QQ号, AuthCode);
        }
    }
}