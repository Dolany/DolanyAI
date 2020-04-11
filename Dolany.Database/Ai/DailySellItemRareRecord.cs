using System;

namespace Dolany.Database.Ai
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
                DateStr = dataStr
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
                DateStr = dataStr
            };
            MongoService<DailySellItemRareRecord>.Insert(record);
            return record;
        }

        public void Update()
        {
            MongoService<DailySellItemRareRecord>.Update(this);
        }
    }

    public class DailySellItemModel
    {
        public string Name { get; set; }

        public int Price { get; set; }

        public string Attr { get; set; }
    }
}
