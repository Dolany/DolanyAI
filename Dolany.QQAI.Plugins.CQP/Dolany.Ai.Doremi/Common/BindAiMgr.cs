using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Common
{
    public class BindAiMgr : IDataMgr
    {
        public static BindAiMgr Instance { get; } = new BindAiMgr();

        public Dictionary<string, BindAiModel> AiDic;

        private BindAiMgr()
        {
            RefreshData();
            //DataRefresher.Instance.Register(this);
        }

        public BindAiModel this[string AiName] => AiDic[AiName];
        public void RefreshData()
        {
            AiDic = CommonUtil.ReadJsonData<Dictionary<string, BindAiModel>>("BindAiData");
        }
    }

    public class BindAiModel
    {
        public long SelfNum { get; set; }

        public string CommandQueue { get; set; }

        public string ClientPath { get; set; }

        public string ImagePath => ClientPath + "temp/image/";

        public string LogPath => ClientPath + "logs/";

        public string VoicePath => ClientPath + "temp/voice/";
    }
}
