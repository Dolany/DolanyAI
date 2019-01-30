using System;

namespace Dolany.Ai.MQ.MahuaApis
{
    using System.Runtime.ExceptionServices;
    using System.Runtime.InteropServices;

    public static class APIEx
    {
        [DllImport("bin\\message.dll")]
        private static extern string Api_GetGroupMemberList(string 群号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_SendPraise(string QQ号, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_163Music(string 歌曲ID, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_Restart(string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern int Api_Ban(string 群号, string QQ, int 禁言时长, string AuthCode);

        [HandleProcessCorruptedStateExceptions]
        public static string GetGroupMemberList(string 群号)
        {
            try
            {
                var AuthCode = Utility.GetAuthCode();
                return Api_GetGroupMemberList(群号, AuthCode);
            }
            catch (Exception ex)
            {
                MahuaModule.RuntimeLogger.Log(ex);
                return string.Empty;
            }
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
                MahuaModule.RuntimeLogger.Log(ex);
                return string.Empty;
            }
        }

        public static string Restart()
        {
            var AuthCode = Utility.GetAuthCode();
            return Api_Restart(AuthCode);
        }

        // 	禁言
        public static int Ban(string 群号, string QQ, int 禁言时长)
        {
            return Api_Ban(群号, QQ, 禁言时长 * 60, Utility.GetAuthCode()); 
        }
    }
}
