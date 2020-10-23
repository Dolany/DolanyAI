using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.WorldLine.Doremi.Cache;
using Dolany.WorldLine.Doremi.Model;

namespace Dolany.WorldLine.Doremi.Ai.Game.Xiuxian
{
    public class MsgCounterSvc
    {
        private static readonly string dataSource = Configger<AIConfigEx>.Instance.AIConfig.MsgCounterCacheDb;

        private const long DialyLimit = 1000;

        public static List<long> GetAllEnabledPersons()
        {
            try
            {
                using var db = new ExCacherContent(dataSource);
                return db.CounterEnableRecord.Select(p => p.QQNum).ToList();
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
                return new List<long>();
            }
        }

        public static void PersonEnable(long QQNum)
        {
            try
            {
                using var db = new ExCacherContent(dataSource);
                if (db.CounterEnableRecord.Any(p => p.QQNum == QQNum))
                {
                    return;
                }

                db.CounterEnableRecord.Add(new CounterEnableRecord() {QQNum = QQNum});
                db.SaveChanges();
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
        }

        public static void PersonDisable(long QQNum)
        {
            try
            {
                using var db = new ExCacherContent(dataSource);
                var record = db.CounterEnableRecord.FirstOrDefault(p => p.QQNum == QQNum);
                if (record == null)
                {
                    return;
                }

                db.CounterEnableRecord.Remove(record);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
        }

        public static void Cache(Dictionary<long, long> RecordDic)
        {
            foreach (var (key, value) in RecordDic)
            {
                Cache(key, value);
            }
        }

        public static void Cache(long QQNum, long count = 1)
        {
            try
            {
                using var db = new ExCacherContent(dataSource);
                var record = db.PersonMsgCountRecord.FirstOrDefault(p => p.QQNum == QQNum);
                if (record == null)
                {
                    record = new PersonMsgCountRecord() {QQNum = QQNum, Count = count};
                    db.PersonMsgCountRecord.Add(record);
                }
                else
                {
                    var todayStr = DateTime.Now.ToString("yyyyMMdd");
                    if (string.IsNullOrEmpty(record.LastDate) || record.LastDate != todayStr)
                    {
                        record.LastDate = todayStr;
                        record.TodayCount = 0;
                    }

                    record.TodayCount ??= 0;

                    if (record.TodayCount >= DialyLimit)
                    {
                        return;
                    }

                    count = Math.Min(count, DialyLimit - record.TodayCount ?? 0);
                    record.TodayCount += count;
                    record.Count += count;
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
        }

        public static void Consume(long QQNum, long count = 1)
        {
            try
            {
                using var db = new ExCacherContent(dataSource);
                var record = db.PersonMsgCountRecord.FirstOrDefault(p => p.QQNum == QQNum);
                if (record == null)
                {
                    return;
                }
                else
                {
                    record.Count = count > record.Count ? 0 : record.Count - count;
                }

                db.SaveChanges();
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
        }

        public static long Get(long QQNum)
        {
            try
            {
                using var db = new ExCacherContent(dataSource);
                var record = db.PersonMsgCountRecord.FirstOrDefault(p => p.QQNum == QQNum);
                return record?.Count ?? 0;
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
                return 0;
            }
        }

        public static void CleanAll()
        {
            try
            {
                using var db = new ExCacherContent(dataSource);
                var records = db.PersonMsgCountRecord.ToArray();
                db.PersonMsgCountRecord.RemoveRange(records);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
        }
    }
}
