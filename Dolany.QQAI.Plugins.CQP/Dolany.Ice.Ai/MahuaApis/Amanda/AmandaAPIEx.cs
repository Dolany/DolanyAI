using System;
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

        [DllImport("bin\\message.dll")]
        private static extern string Api_163Music(string 歌曲ID, string AuthCode);

        [DllImport("bin\\message.dll")]
        private static extern string Api_Restart(string AuthCode);

        private static string GetGroupMemberList(string 群号)
        {
            try
            {
                var AuthCode = Utility.GetAuthCode();
                return Api_GetGroupMemberList(群号, AuthCode);
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex.Message + '\r' + ex.StackTrace);
                return "";
            }
        }

        public static GroupMemberListViewModel GetMemberInfos(long GroupNum)
        {
            var ml = GetGroupMemberList(GroupNum.ToString());
            if (string.IsNullOrEmpty(ml))
            {
                return null;
            }
            return JsonHelper.DeserializeJsonToObject<GroupMemberListViewModel>(ml);
        }

        public static int SendPraise(string QQ号)
        {
            var AuthCode = Utility.GetAuthCode();
            return Api_SendPraise(QQ号, AuthCode);
        }

        // 	网易云点歌
        public static string _163Music(string 歌曲ID)
        {
            var AuthCode = Utility.GetAuthCode();
            return Api_163Music(歌曲ID, AuthCode);
        }

        public static string Restart()
        {
            var AuthCode = Utility.GetAuthCode();
            return Api_Restart(AuthCode);
        }
    }
}