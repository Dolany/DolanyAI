using System;
using System.Collections.Generic;

namespace Dolany.Database.Ai
{
    public class DailyLimitRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<string, DailyLimitCommand> Commands { get; set; } = new Dictionary<string, DailyLimitCommand>();

        public static DailyLimitRecord Get(long QQNum)
        {
            var record = MongoService<DailyLimitRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new DailyLimitRecord(){QQNum = QQNum};
            MongoService<DailyLimitRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            MongoService<DailyLimitRecord>.Update(this);
        }

        public bool Check(string command, int times)
        {
            if (!Commands.ContainsKey(command))
            {
                return true;
            }

            var refreshTime = DateTime.Now.Hour > 4 ? DateTime.Now.Date.AddHours(4) : DateTime.Now.Date.AddHours(4).AddDays(-1);
            var dailyLimitCommand = Commands[command];
            if (dailyLimitCommand.LastTime == null || dailyLimitCommand.LastTime.Value < refreshTime)
            {
                return true;
            }

            return dailyLimitCommand.Times < times;
        }

        public void Cache(string command)
        {
            if (!Commands.ContainsKey(command))
            {
                Commands.Add(command, new DailyLimitCommand(){Times = 1, LastTime = DateTime.Now});
                return;
            }

            var refreshTime = DateTime.Now.Hour > 4 ? DateTime.Now.Date.AddHours(4) : DateTime.Now.Date.AddHours(4).AddDays(-1);
            var dailyLimitCommand = Commands[command];
            if (dailyLimitCommand.LastTime == null || dailyLimitCommand.LastTime.Value < refreshTime)
            {
                dailyLimitCommand.Times = 1;
            }
            else
            {
                dailyLimitCommand.Times++;
            }
            dailyLimitCommand.LastTime = DateTime.Now;
        }

        public void Decache(string command, int count = 1)
        {
            if (!Commands.ContainsKey(command))
            {
                Commands.Add(command, new DailyLimitCommand(){Times = -count, LastTime = DateTime.Now});
                return;
            }

            var refreshTime = DateTime.Now.Hour > 4 ? DateTime.Now.Date.AddHours(4) : DateTime.Now.Date.AddHours(4).AddDays(-1);
            var dailyLimitCommand = Commands[command];
            if (dailyLimitCommand.LastTime == null || dailyLimitCommand.LastTime.Value < refreshTime)
            {
                dailyLimitCommand.Times = -count;
            }
            else
            {
                dailyLimitCommand.Times -= count;
            }
            dailyLimitCommand.LastTime = DateTime.Now;
        }
    }

    public class DailyLimitCommand
    {
        public int Times { get; set; }

        public DateTime? LastTime { get; set; }
    }
}
