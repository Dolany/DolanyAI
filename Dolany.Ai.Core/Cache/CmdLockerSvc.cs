using System;
using Dolany.Ai.Common;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Cache
{
    public class CmdLockerSvc : IDependency
    {
        public static bool TryLock(long QQNum, string Cmd, TimeSpan ExpirySpan)
        {
            if (CmdLockRec.IsLocked(QQNum, Cmd))
            {
                return false;
            }

            CmdLockRec.Insert(QQNum, Cmd, ExpirySpan);
            return true;
        }

        public static void ReleaseLock(long QQNum, string Cmd)
        {
            CmdLockRec.ReleaseLock(QQNum, Cmd);
        }
    }

    public class CmdLockRec : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string Cmd { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExpiryTime { get; set; }

        public static bool IsLocked(long QQNum, string Cmd)
        {
            return MongoService<CmdLockRec>.Count(p => p.QQNum == QQNum && p.Cmd == Cmd) > 0;
        }

        public static void Insert(long QQNum, string Cmd, TimeSpan ExpirySpan)
        {
            MongoService<CmdLockRec>.Insert(new CmdLockRec()
            {
                QQNum = QQNum,
                Cmd = Cmd,
                ExpiryTime = DateTime.Now.Add(ExpirySpan)
            });
        }

        public static void ReleaseLock(long QQNum, string Cmd)
        {
            MongoService<CmdLockRec>.DeleteMany(p => p.QQNum == QQNum && p.Cmd == Cmd);
        }
    }
}
