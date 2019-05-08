using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Sys.BonusCenter
{
    public class AutoBonusRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string Code { get; set; }

        public static bool Check(long QQNum, string Code)
        {
            return MongoService<AutoBonusRecord>.GetOnly(p => p.QQNum == QQNum && p.Code == Code) == null;
        }

        public static void Record(long QQNum, string Code)
        {
            MongoService<AutoBonusRecord>.Insert(new AutoBonusRecord(){QQNum = QQNum, Code = Code});
        }
    }
}
