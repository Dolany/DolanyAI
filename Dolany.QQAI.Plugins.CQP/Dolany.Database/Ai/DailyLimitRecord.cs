using System;
using System.Collections.Generic;
using Dolany.Ai.Common;

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

            var dailyLimitCommand = Commands[command];
            if (dailyLimitCommand.ExpiryTime == null || dailyLimitCommand.ExpiryTime.Value.ToLocalTime() < DateTime.Now)
            {
                return true;
            }

            return dailyLimitCommand.Times < times;
        }

        public void Cache(string command)
        {
            if (!Commands.ContainsKey(command))
            {
                Commands.Add(command, new DailyLimitCommand(){Times = 1, ExpiryTime = CommonUtil.UntilTommorow()});
                return;
            }

            var dailyLimitCommand = Commands[command];
            if (dailyLimitCommand.ExpiryTime == null || dailyLimitCommand.ExpiryTime.Value.ToLocalTime() < DateTime.Now)
            {
                dailyLimitCommand.Times = 1;
            }
            else
            {
                dailyLimitCommand.Times++;
            }
            dailyLimitCommand.ExpiryTime = CommonUtil.UntilTommorow();
        }

        public void Decache(string command, int count = 1)
        {
            if (!Commands.ContainsKey(command))
            {
                Commands.Add(command, new DailyLimitCommand(){Times = -count, ExpiryTime = CommonUtil.UntilTommorow()});
                return;
            }

            var dailyLimitCommand = Commands[command];
            if (dailyLimitCommand.ExpiryTime == null || dailyLimitCommand.ExpiryTime.Value.ToLocalTime() < DateTime.Now)
            {
                dailyLimitCommand.Times = -count;
            }
            else
            {
                dailyLimitCommand.Times -= count;
            }

            dailyLimitCommand.ExpiryTime = CommonUtil.UntilTommorow();
        }
    }

    public class DailyLimitCommand
    {
        public string Command { get; set; }

        public int Times { get; set; }

        public DateTime? ExpiryTime { get; set; }
    }
}
