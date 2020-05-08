using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchDailyScene : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string DateStr { get; set; }

        public List<string> Scenes { get; set; } = new List<string>();

        public static ArchDailyScene Get(long QQNum)
        {
            var todayStr = DateTime.Today.ToString("yyyyMMdd");
            var rec = MongoService<ArchDailyScene>.GetOnly(p => p.QQNum == QQNum && p.DateStr == todayStr);
            if (rec != null)
            {
                return rec;
            }

            var svc = AutofacSvc.Resolve<ArchaeologySceneSvc>();
            rec = new ArchDailyScene() {QQNum = QQNum, DateStr = todayStr, Scenes = svc.RandScenes(5).Select(p => p.Name).ToList()};
            MongoService<ArchDailyScene>.Insert(rec);
            return rec;
        }
    }
}
