using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Common
{
    public class BindAiSvc : IDataMgr, IDependency
    {
        public Dictionary<string, BindAiModel> AiDic;

        public IEnumerable<long> AllAiNums => AiDic.Values.Select(p => p.SelfNum).ToArray();

        public BindAiModel this[string AiName] => AiDic.ContainsKey(AiName) ? AiDic[AiName] : null;
        public BindAiModel this[long AiNum] => AiDic.Values.FirstOrDefault(p => p.SelfNum == AiNum);
        public void RefreshData()
        {
            AiDic = CommonUtil.ReadJsonData_NamedList<BindAiModel>("BindAiData").ToDictionary(p => p.Name, p => p);
        }
    }

    public class BindAiModel : INamedJsonModel
    {
        public long SelfNum { get; set; }

        public string ClientPath { get; set; }

        public string ImagePath => ClientPath + "temp/image/";

        public string LogPath => ClientPath + "logs/";

        public string VoicePath => ClientPath + "temp/voice/";

        public string Name { get; set; }

        public bool IsConnected { get; set; } = true;
    }
}
