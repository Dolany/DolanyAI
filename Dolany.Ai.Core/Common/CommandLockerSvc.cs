using System;
using System.Collections.Concurrent;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Common
{
    public class CommandLockerSvc : IDependency
    {
        private readonly ConcurrentDictionary<string, LockUnit> LockUnits = new ConcurrentDictionary<string, LockUnit>();

        public bool Check(long QQNum, string Command)
        {
            return LockUnits.All(u => u.Value.QQNum != QQNum || !u.Value.LockCommands.Contains(Command));
        }

        public string Lock(long QQNum, string[] Commands)
        {
            var guid = Guid.NewGuid().ToString();
            LockUnits.TryAdd(guid, new LockUnit() {QQNum = QQNum, LockCommands = Commands});
            return guid;
        }

        public void FreeLock(string lockKey)
        {
            LockUnits.TryRemove(lockKey, out _);
        }
    }

    public class LockUnit
    {
        public long QQNum { get; set; }

        public string[] LockCommands { get; set; }
    }
}
