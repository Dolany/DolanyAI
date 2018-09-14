using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class TulingImportRequestData
    {
        public string apikey { get; set; }

        public TulingImportRequestDataData data { get; set; }
    }

    public class TulingImportRequestDataData
    {
        public IEnumerable<TulingImportRequestDataQA> list;
    }

    public class TulingImportRequestDataQA
    {
        public string question { get; set; }
        public string answer { get; set; }
    }
}