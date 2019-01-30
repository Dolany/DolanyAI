using System;
using System.Collections.Generic;

namespace Dolany.Game.Engine
{
    public class ScriptModel
    {
        public Dictionary<string, object> DataDic;

        public ScriptModel(string scriptName, Action<string> ErrorCallBack)
        {
            // todo
        }

        public void RestoreData(Dictionary<string, object> DataDic)
        {
            this.DataDic = DataDic;
        }
    }
}
