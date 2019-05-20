namespace Dolany.Database.Ai
{
    public class GroupNicknameRecord : DbBaseEntity
    {
        public long GroupNum { get;set; }

        public long QQNum { get; set; }

        public string Nickname { get; set; }

        public static GroupNicknameRecord Get(long GroupNum, long QQNum)
        {
            return MongoService<GroupNicknameRecord>.GetOnly(p => p.GroupNum == GroupNum && p.QQNum == QQNum);
        }
    }
}
