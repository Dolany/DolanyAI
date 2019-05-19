﻿using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Common
{
    public class BindAiMgr
    {
        public static BindAiMgr Instance { get; } = new BindAiMgr();

        private readonly Dictionary<string, BindAiModel> AiDic;

        private BindAiMgr()
        {
            AiDic = CommonUtil.ReadJsonData<Dictionary<string, BindAiModel>>("BindAiData");
        }

        public BindAiModel this[string AiName] => AiDic[AiName];
    }

    public class BindAiModel
    {
        public long SelfNum { get; set; }

        public string CommandQueue { get; set; }
    }
}
