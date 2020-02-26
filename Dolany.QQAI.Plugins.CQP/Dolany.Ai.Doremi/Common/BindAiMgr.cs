﻿using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Common
{
    public class BindAiMgr : IDataMgr
    {
        public Dictionary<string, BindAiModel> AiDic;

        private static DataRefresher DataRefresher => AutofacSvc.Resolve<DataRefresher>();

        public BindAiMgr()
        {
            RefreshData();
            DataRefresher.Register(this);
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
