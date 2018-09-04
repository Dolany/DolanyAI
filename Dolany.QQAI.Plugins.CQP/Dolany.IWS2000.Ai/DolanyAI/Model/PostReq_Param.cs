using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.IWS2000.Ai.DolanyAI
{
    /// <summary>
    /// post请求
    /// </summary>
    public class PostReq_Param
    {
        public string InterfaceName
        {
            get;
            set;
        }

        public object data
        {
            get;
            set;
        }
    }
}