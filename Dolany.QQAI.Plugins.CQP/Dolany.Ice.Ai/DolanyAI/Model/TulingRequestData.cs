using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class TulingRequestData
    {
        public int reqType { get; set; }
        public perceptionData perception { get; set; }
        public userInfoData userInfo { get; set; }
    }

    public class perceptionData
    {
        public inputTextData inputText { get; set; }
        public inputImageData inputImage { get; set; }
        public inputMediaData inputMedia { get; set; }
        public selfInfoData selfInfo { get; set; }
    }

    public class inputTextData
    {
        public string text { get; set; }
    }

    public class inputImageData
    {
        public string url { get; set; }
    }

    public class inputMediaData
    {
        public string url { get; set; }
    }

    public class selfInfoData
    {
        public locationData location { get; set; }
    }

    public class locationData
    {
        public string city { get; set; }
        public string province { get; set; }
        public string street { get; set; }
    }

    public class userInfoData
    {
        public string apiKey { get; set; }
        public string userId { get; set; }
        public string groupId { get; set; }
        public string userIdName { get; set; }
    }
}