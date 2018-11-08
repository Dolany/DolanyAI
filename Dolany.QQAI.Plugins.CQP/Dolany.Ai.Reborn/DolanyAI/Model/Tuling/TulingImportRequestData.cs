using System.Collections.Generic;

namespace Dolany.Ai.Reborn.DolanyAI.Model.Tuling
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
