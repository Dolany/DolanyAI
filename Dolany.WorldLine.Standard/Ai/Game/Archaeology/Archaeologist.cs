using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    /// <summary>
    /// 考古学家状态记录
    /// </summary>
    public class Archaeologist : DbBaseEntity
    {
        public long QQNum { get; set; }

        /// <summary>
        /// SAN值上限
        /// </summary>
        public int SAN { get; set; } = 100;

        /// <summary>
        /// 当前SAN值
        /// </summary>
        public int CurSAN { get; set; } = 100;

        /// <summary>
        /// 寒冰元素
        /// </summary>
        public int Ice { get; set; } = 1;

        /// <summary>
        /// 火焰元素
        /// </summary>
        public int Flame { get; set; } = 1;

        /// <summary>
        /// 雷电元素
        /// </summary>
        public int Lightning { get; set; } = 1;

        /// <summary>
        /// 复活时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RebornTime { get; set; } = DateTime.Now;

        /// <summary>
        /// SAN值刷新时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SANRefreshTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否处于死亡状态
        /// </summary>
        public bool IsDead => RebornTime.HasValue && RebornTime > DateTime.Now;

        public static Archaeologist Get(long QQNum)
        {
            var rec = MongoService<Archaeologist>.GetOnly(p => p.QQNum == QQNum);
            if (rec != null)
            {
                if (rec.RefreshSAN())
                {
                    rec.Update();
                }

                return rec;
            }

            rec = new Archaeologist(){QQNum = QQNum};
            MongoService<Archaeologist>.Insert(rec);
            return rec;
        }

        /// <summary>
        /// 尝试刷新SAN值
        /// </summary>
        /// <returns></returns>
        public bool RefreshSAN()
        {
            if (SANRefreshTime.Date == DateTime.Today)
            {
                return false;
            }

            CurSAN = SAN;
            SANRefreshTime = DateTime.Now;
            return true;
        }

        public void Update()
        {
            MongoService<Archaeologist>.Update(this);
        }
    }
}
