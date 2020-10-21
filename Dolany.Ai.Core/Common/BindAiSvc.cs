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

    /// <summary>
    /// 终端机器人模型
    /// </summary>
    public class BindAiModel : INamedJsonModel
    {
        /// <summary>
        /// 机器人QQ号
        /// </summary>
        public long SelfNum { get; set; }

        /// <summary>
        /// 客户端根目录
        /// </summary>
        public string ClientPath { get; set; }

        /// <summary>
        /// 图片缓存地址
        /// </summary>
        public string ImagePath => ClientPath + "temp/image/";

        /// <summary>
        /// 日志缓存地址
        /// </summary>
        public string LogPath => ClientPath + "logs/";

        /// <summary>
        /// 音频文件缓存地址
        /// </summary>
        public string VoicePath => ClientPath + "temp/voice/";

        /// <summary>
        /// 机器人代码
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsConnected { get; set; } = true;
    }
}
