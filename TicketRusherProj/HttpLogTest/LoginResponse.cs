using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLogTest
{
    public class LoginResponse
    {
        public bool hasError { get; set; }

        public LoginResponseContent content { get; set; }
        public IEnumerable<LoginResponseError> errors { get; set; }
    }

    public class LoginResponseError
    {
        public int code { get; set; }
        public string field { get; set; }
    }

    public class LoginResponseContent
    {
        public bool success { get; set; }
        public int status { get; set; }
        public LoginResponseData data { get; set; }
    }

    public class LoginResponseData
    {
        public string titleMsg { get; set; }
        public int resultCode { get; set; }
    }
}