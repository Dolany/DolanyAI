using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class TulingImportResponseData
    {
        public TulingImportResponseDataData data { get; set; }

        public int code { get; set; }
    }

    public class TulingImportResponseDataData
    {
        public int successNum { get; set; }
        public IEnumerable<TulingImportResponseDataKnowledge> knowledgeList { get; set; }
    }

    public class TulingImportResponseDataKnowledge
    {
        public int id { get; set; }
        public string time { get; set; }
        public string question { get; set; }
        public string label_id { get; set; }
        public string answer { get; set; }
    }
}