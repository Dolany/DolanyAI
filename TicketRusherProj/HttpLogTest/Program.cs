using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLogTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var response = RequestHelper.PostData<LoginResponse>(new PostReq_Param
            {
                InterfaceName = "https://ipassport.damai.cn/newlogin/login.do",
                data = new LoginFormData
                {
                    keepLogin = true,
                    loginId = "dolany@sina.cn",
                    password2 = "6cce242b2fb9166ed7fed50a0fe4dfd24e9182829afdca2398af1a6e727714957d724533066d643bc3ed32ce3375553a12c7ac6f12979f7f6214739a06aab6f6e29d571b6ece7e5f3ba47a5897574ce39ee1aa104c5646f92f5f19c03339dffdc1b38b81636aa90b718c3d3787c89c0ee2ceeb1501061af5da65839900a7589e"
                }
            });
        }
    }
}