﻿namespace Dolany.Ai.Reborn.MahuaApis
{
    using System;
    using System.Runtime.ExceptionServices;
    using System.Runtime.InteropServices;

    using Dolany.Ai.Reborn.DolanyAI.Common;
    using Dolany.Ai.Reborn.DolanyAI.Net;
    using Dolany.Ai.Reborn.MahuaApis.ViewModel;

    public static class AmandaAPIEx
    {
        [DllImport("bin\\message.dll")]
        private static extern string Api_GetGroupMemberList(string 群号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SendPraise(string QQ号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_163Music(string 歌曲ID, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_Restart(string AuthCode);

        [HandleProcessCorruptedStateExceptions]
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

        public static void SendPraise(string QQ号)
        {
            var AuthCode = Utility.GetAuthCode();
            Api_SendPraise(QQ号, AuthCode);
        }

        // 	网易云点歌
        [HandleProcessCorruptedStateExceptions]
        public static string _163Music(string 歌曲ID)
        {
            try
            {
                var AuthCode = Utility.GetAuthCode();
                return Api_163Music(歌曲ID, AuthCode);
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                return string.Empty;
            }
        }

        public static string Restart()
        {
            var AuthCode = Utility.GetAuthCode();
            return Api_Restart(AuthCode);
        }
    }
}