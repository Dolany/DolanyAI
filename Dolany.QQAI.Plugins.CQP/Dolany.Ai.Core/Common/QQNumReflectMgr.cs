using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Common
{
    public class QQNumReflectMgr : IDataMgr, IDependency
    {
        private Dictionary<long, string> RelectDic;

        public string this[long QQNum] => RelectDic.ContainsKey(QQNum) ? RelectDic[QQNum] : QQNum.ToString();

        public void RefreshData()
        {
            RelectDic = CommonUtil.ReadJsonData<Dictionary<string, string>>("QQNumReflectData").ToDictionary(p => long.Parse(p.Key), p => p.Value);
        }
    }
}
