using Dolany.Database.Sqlite;
using System;

namespace Dolany.Ai.Core.Common
{
    public class AliveStateMgr
    {
        public static AliveStateCache GetState(long GroupNum, long QQNum)
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
