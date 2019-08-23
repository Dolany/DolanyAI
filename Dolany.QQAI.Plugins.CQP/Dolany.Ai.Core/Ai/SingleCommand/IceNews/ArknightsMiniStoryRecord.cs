using Dolany.Ai.Core.Cache;
using Dolany.Database;
using System;

namespace Dolany.Ai.Core.Ai.SingleCommand.IceNews
{
    public class ArknightsMiniStoryRecord : DbBaseEntity
    {
        public int No { get; set; }

        public string Path { get; set; }

        public DateTime UpdateTime { get; set; }

        public static ArknightsMiniStoryRecord Get(int No)
        {
            return MongoService<ArknightsMiniStoryRecord>.GetOnly(p => p.No == No);
        }

        public static ArknightsMiniStoryRecord GetLast()
        {
            var lastestNoRec = GlobalVarRecord.Get("ArknightsMiniStoryNo");
            if (string.IsNullOrEmpty(lastestNoRec.Value))
            {
                lastestNoRec.Value = "0";
            }

            var lastNo = int.Parse(lastestNoRec.Value);
            return Get(lastNo);
        }

        public static void Insert(string Path)
        {
            var lastestNoRec = GlobalVarRecord.Get("ArknightsMiniStoryNo");
            if (string.IsNullOrEmpty(lastestNoRec.Value))
            {
                lastestNoRec.Value = "0";
            }

            var lastNo = int.Parse(lastestNoRec.Value);
            var newRecord = new ArknightsMiniStoryRecord()
            {
                No = lastNo + 1,
                Path = Path,
                UpdateTime = DateTime.Now
            };
            MongoService<ArknightsMiniStoryRecord>.Insert(newRecord);

            lastestNoRec.Value = newRecord.No.ToString();
            lastestNoRec.Update();
        }
    }
}
