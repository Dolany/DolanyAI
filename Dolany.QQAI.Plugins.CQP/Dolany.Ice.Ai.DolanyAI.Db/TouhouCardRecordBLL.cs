using System;
using System.IO;
using System.Linq;
using Dolany.Ice.Ai.DolanyAI.Utils;

namespace Dolany.Ice.Ai.DolanyAI.Db
{
    public static class TouhouCardRecordBLL
    {
        private const string PicPath = "TouhouCard/";

        public static string RandomCard(long FromQQ)
        {
            using (var db = new AIDatabase())
            {
                var query = db.TouhouCardRecord.Where(p => p.QQNum == FromQQ);
                if (query.IsNullOrEmpty())
                {
                    var tcr = new TouhouCardRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        UpdateDate = DateTime.Now.Date,
                        CardName = GetRandCard(),
                        QQNum = FromQQ
                    };
                    db.TouhouCardRecord.Add(tcr);
                    db.SaveChanges();

                    return PicPath + tcr.CardName;
                }

                var rec = query.First();
                if (rec.UpdateDate >= DateTime.Now.Date)
                {
                    return PicPath + rec.CardName;
                }

                rec.CardName = GetRandCard();
                rec.UpdateDate = DateTime.Now.Date;
                db.SaveChanges();

                return PicPath + rec.CardName;
            }
        }

        private static string GetRandCard()
        {
            var dir = new DirectoryInfo(PicPath);
            var files = dir.GetFiles();
            var rIdx = Utility.RandInt(files.Length);
            return files.ElementAt(rIdx).Name;
        }
    }
}