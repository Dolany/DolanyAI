using System;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Doremi.Common
{
    public class AliveStateSvc
    {
        public AliveStateCache GetState(long GroupNum, long QQNum)
        {
            var key = $"AliveState-{GroupNum}-{QQNum}";
            return SCacheService.Get<AliveStateCache>(key);
        }
    }

    public class AliveStateCache
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public DateTime RebornTime { get; set; }

        public string Name { get; set; }
    }
}
