using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Game.WeaponWar
{
    public class WWSMHelper
    {
        private List<SMModel> SMList;

        public static WWSMHelper Instance { get; } = new WWSMHelper();

        private WWSMHelper()
        {
            var dic = CommonUtil.ReadJsonData<Dictionary<string, SMModel>>("SMData");
            SMList = dic.Select(sm =>
            {
                var (key, value) = sm;
                value.Name = key;
                return value;
            }).ToList();
        }
    }

    public class SMModel
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Price { get; set; }
    }
}
