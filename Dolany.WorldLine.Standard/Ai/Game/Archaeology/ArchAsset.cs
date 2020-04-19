using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchAsset : DbBaseEntity
    {
        public long QQNum { get; set; }

        /// <summary>
        /// 翠绿琥珀
        /// </summary>
        public int GreenAmbur { get; set; } = 2;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime GreenAmburRefreshTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 碧蓝琥珀
        /// </summary>
        public int BlueAmbur { get; set; }

        /// <summary>
        /// 墨玉
        /// </summary>
        public int BlackJade { get; set; }

        /// <summary>
        /// 赤星石
        /// </summary>
        public int RedStarStone { get; set; }

        /// <summary>
        /// 寒冰元素精魄
        /// </summary>
        public int IceEssence { get; set; }

        /// <summary>
        /// 火焰元素精魄
        /// </summary>
        public int FlameEssence { get; set; }

        /// <summary>
        /// 雷电元素精魄
        /// </summary>
        public int LightningEssence { get; set; }

        /// <summary>
        /// 刷新翠绿琥珀
        /// </summary>
        /// <returns></returns>
        public bool RefreshGreenAmbur()
        {
            if (GreenAmburRefreshTime.Date == DateTime.Today)
            {
                return false;
            }

            GreenAmbur = 2;
            GreenAmburRefreshTime = DateTime.Now;
            return true;
        }

        public static ArchAsset Get(long QQNum)
        {
            var rec = MongoService<ArchAsset>.GetOnly(p => p.QQNum == QQNum);
            if (rec != null)
            {
                if (rec.RefreshGreenAmbur())
                {
                    rec.Update();
                }

                return rec;
            }

            rec = new ArchAsset(){QQNum = QQNum};
            MongoService<ArchAsset>.Insert(rec);
            return rec;
        }

        public bool ConsumeAmbur()
        {
            if (GreenAmbur > 0)
            {
                GreenAmbur--;
                return true;
            }

            if (BlueAmbur <= 0)
            {
                return false;
            }

            BlueAmbur--;
            return true;
        }

        public void Update()
        {
            MongoService<ArchAsset>.Update(this);
        }
    }
}
