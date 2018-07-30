using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class TulingResponseData
    {
        public TulingResponseData_Intent intent { get; set; }
        public IEnumerable<TulingResponseData_Result> results { get; set; }
    }

    public class TulingResponseData_Intent
    {
        public int code { get; set; }
        public string intentName { get; set; }
        public string actionName { get; set; }
        public IEnumerable<KeyValuePair<string, string>> parameters { get; set; }
    }

    public class TulingResponseData_Result
    {
        public string resultType { get; set; }
        public TulingResponseData_Value values { get; set; }
        public int groupType { get; set; }
    }

    public class TulingResponseData_Value
    {
        public string text { get; set; }
    }
}