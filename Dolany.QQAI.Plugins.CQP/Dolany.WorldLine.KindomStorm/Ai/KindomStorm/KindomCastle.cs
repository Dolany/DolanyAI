using System.Collections.Generic;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Common;
using Dolany.Database;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class KindomCastle : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string CharactorName { get; set; }

        public string CastleName { get; set; }

        public int Golds { get; set; }

        /// <summary>
        /// 粮草
        /// </summary>
        public int Commissariat { get; set; }

        public Dictionary<string, int> Buildings { get; set; } = new Dictionary<string, int>();

        public static KindomCastle Get(long GroupNum, long QQNum)
        {
            var castle = MongoService<KindomCastle>.GetOnly(p => p.QQNum == QQNum);
            if (castle != null)
            {
                return castle;
            }

            var group = GroupSettingMgr.Instance[GroupNum];
            var info = APIEx.GetQQInfo(QQNum, group.BindAi);
            castle = new KindomCastle()
            {
                QQNum = QQNum, CharactorName = info.nick, CastleName = $"{info.nick}的城堡", Buildings = new Dictionary<string, int>()
                {
                    {"城镇", 1},
                    {"粮仓", 1}
                }
            };
            MongoService<KindomCastle>.Insert(castle);
            return castle;
        }

        public void Update()
        {
            MongoService<KindomCastle>.Update(this);
        }
    }
}
