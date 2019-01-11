namespace Dolany.Database.Ai
{
    using System;

    public class MemberRoleCache : BaseEntity
    {
        public long QQNum { get; set; }
        public long GroupNum { get; set; }

        public int Role { get; set; } = 2;
        public DateTime Datatime { get; set; } = DateTime.Now;
        public string Nickname { get; set; }
    }
}
