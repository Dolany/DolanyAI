using System;
using System.Collections.Generic;
using System.Text;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Common
{
    public class BindAiMgr
    {
        public static BindAiMgr Instance { get; } = new BindAiMgr();

        public Dictionary<string, BindAiModel> AiDic;

        private BindAiMgr()
        {
            AiDic = CommonUtil.ReadJsonData<Dictionary<string, BindAiModel>>("BindAiData");
        }
    }

    public class BindAiModel
    {
        public long SelfNum { get; set; }

        public string CommandQueue { get; set; }
    }
}
