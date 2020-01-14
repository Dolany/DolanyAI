using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Dolany.Database;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Standard.Ai.Game.Gift
{
    public class GiftMgr
    {
        public static GiftMgr Instance { get; } = new GiftMgr();

        public List<GiftModel> GiftList { get; }

        private GiftMgr()
        {
            GiftList = CommonUtil.ReadJsonData_NamedList<GiftModel>("GiftData");
        }

        public GiftModel this[string GiftName] => GiftList.FirstOrDefault(p => p.Name == GiftName);

        public IEnumerable<GiftModel> SellingGifts
        {
            get
            {
                const string Key = "SellingGifts";
                var record = MongoService<GlobalVarRecord>.GetOnly(p => p.Key == Key);
                if (record != null)
                {
                    var giftNames = JsonConvert.DeserializeObject<List<string>>(record.Value);
                    return GiftList.Where(p => giftNames.Contains(p.Name)).ToList();
                }

                var gifts = RandomGifts(7);
                record = new GlobalVarRecord()
                {
                    Key = Key,
                    Value = JsonConvert.SerializeObject(gifts),
                    ExpiryTime = CommonUtil.UntilTommorow()
                };
                MongoService<GlobalVarRecord>.Insert(record);

                return GiftList.Where(p => gifts.Contains(p.Name)).ToList();
            }
        }

        private List<string> RandomGifts(int count)
        {
            return Rander.RandSort(GiftList.Select(p => p.Name).ToArray()).Take(count).ToList();
        }
    }

    public class GiftModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<string, int> MaterialDic { get; set; }

        public int GoldNeed { get; set; }

        /// <summary>
        /// 羁绊值
        /// </summary>
        public int Intimate { get; set; }

        /// <summary>
        /// 魅力值
        /// </summary>
        public int Glamour { get; set; }

        public bool Check(Dictionary<string, int> Mas, int Golds, out string msg)
        {
            msg = string.Empty;
            var result = true;
            msg += $"金币：{Golds}/{GoldNeed}\r";
            if (Golds < GoldNeed)
            {
                result = false;
            }

            foreach (var (key, value) in MaterialDic)
            {
                if (!Mas.ContainsKey(key))
                {
                    msg += $"{key}：0/{value}\r";
                    result = false;
                    continue;
                }

                msg += $"{key}：{Mas[key]}/{value}\r";
                if (Mas[key] < value)
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
