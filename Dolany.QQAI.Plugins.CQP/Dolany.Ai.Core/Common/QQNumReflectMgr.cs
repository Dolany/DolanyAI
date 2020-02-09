using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Common
{
    public class QQNumReflectMgr : IDataMgr
    {
        public static QQNumReflectMgr Instance { get; } = new QQNumReflectMgr();

        private Dictionary<long, string> RelectDic;

        public string this[long QQNum] => RelectDic.ContainsKey(QQNum) ? RelectDic[QQNum] : QQNum.ToString();

        private QQNumReflectMgr()
        {
            RefreshData();
            DataRefresher.Instance.Register(this);
        }

        public void RefreshData()
        {
            RelectDic = CommonUtil.ReadJsonData<Dictionary<string, string>>("QQNumReflectData").ToDictionary(p => long.Parse(p.Key), p => p.Value);
        }
    }
}
