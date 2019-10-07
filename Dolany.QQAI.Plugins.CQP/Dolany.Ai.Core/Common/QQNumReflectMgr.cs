using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Common
{
    public class QQNumReflectMgr
    {
        public static QQNumReflectMgr Instance { get; } = new QQNumReflectMgr();

        private readonly Dictionary<long, string> RelectDic;

        public string this[long QQNum] => RelectDic.ContainsKey(QQNum) ? RelectDic[QQNum] : QQNum.ToString();

        private QQNumReflectMgr()
        {
            RelectDic = CommonUtil.ReadJsonData<Dictionary<string, string>>("QQNumReflectData").ToDictionary(p => long.Parse(p.Key), p => p.Value);
        }
    }
}
