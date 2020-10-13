using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Common
{
    /// <summary>
    /// BindAi服务
    /// </summary>
    public class BindAiSvc : IDependency, IDataMgr
    {
        /// <summary>
        /// 所有BindAi的字典
        /// </summary>
        public Dictionary<string, BindAiModel> AiDic;

        /// <summary>
        /// 所有BindAi的QQ号
        /// </summary>
        public IEnumerable<long> AllAiNums => AiDic.Values.Select(p => p.SelfNum).ToArray();

        /// <summary>
        /// 根据AI名获取BindAi
        /// </summary>
        /// <param name="AiName"></param>
        public BindAiModel this[string AiName] => AiDic.ContainsKey(AiName) ? AiDic[AiName] : null;
        /// <summary>
        /// 根据Ai QQ号获取BindAi
        /// </summary>
        /// <param name="AiNum"></param>
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
