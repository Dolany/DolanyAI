using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.UtilityTool;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.Shopping
{
    public class DailySellItemRareRecord : DbBaseEntity
    {
        public string DateStr { get; set; }

        public int Hour { get; set; }

        public DailySellItemModel[] Items { get; set; }

        public bool IsActive => !IsOver && !IsBefore;

        public bool IsBefore => DateTime.Now.Hour < Hour;

        public bool IsOver => Hour + 3 <= DateTime.Now.Hour;

        public static DailySellItemRareRecord GetToday()
        {
            var dataStr = DateTime.Now.ToString("yyyyMMdd");
            var record = MongoService<DailySellItemRareRecord>.GetOnly(p => p.DateStr == dataStr);

            if (record != null)
            {
                return record;
            }

            record = new DailySellItemRareRecord()
            {
                DateStr = dataStr,
                Hour = 6 + Rander.RandInt(16),
                Items = CreateDailySellItems_Rare()
            };
            MongoService<DailySellItemRareRecord>.Insert(record);
            return record;
        }

        public static DailySellItemRareRecord GetTomorrow()
        {
            var dataStr = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            var record = MongoService<DailySellItemRareRecord>.GetOnly(p => p.DateStr == dataStr);

            if (record != null)
            {
                return record;
            }

            record = new DailySellItemRareRecord()
            {
                DateStr = dataStr,
                Hour = 6 + Rander.RandInt(16),
                Items = CreateDailySellItems_Rare()
            };
            MongoService<DailySellItemRareRecord>.Insert(record);
            return record;
        }

        public void Update()
        {
            MongoService<DailySellItemRareRecord>.Update(this);
        }

        private static DailySellItemModel[] CreateDailySellItems_Rare()
        {
            var honors = AutofacSvc.Resolve<HonorSvc>().HonorList.Where(h => !h.IsLimit);
            var items = honors.SelectMany(h => h.Items).Where(p => p.Price >= 500).ToArray();
            var randSort = Rander.RandSort(items).Take(5);
            return randSort.Select(rs => new DailySellItemModel
            {
                Name = rs.Name,
                Price = rs.Price * 2,
                Attr = string.Join(",", rs.Attributes)
            }).ToArray();
        }
    }

    public class DailySellItemModel
    {
        public string Name { get; set; }

        public int Price { get; set; }

        public string Attr { get; set; }
    }
}
