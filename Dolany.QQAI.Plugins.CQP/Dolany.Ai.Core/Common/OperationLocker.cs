using System.Collections.Generic;

namespace Dolany.Ai.Core.Common
{
    public class OperationLocker
    {
        private static readonly object LockObj = new object();

        private static readonly List<long> LockedList = new List<long>();

        public static bool Lock(long qqNum)
        {
            lock (LockObj)
            {
                if (LockedList.Contains(qqNum))
                {
                    return false;
                }

                LockedList.Add(qqNum);
                return true;
            }
        }

        public static void Unlock(long qqNum)
        {
            lock (LockObj)
            {
                LockedList.Remove(qqNum);
            }
        }
    }
}
