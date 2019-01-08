namespace Dolany.Ai.Core.Db
{
    using System;

    public class MemberRoleCache
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long QQNum { get; set; }
        public long GroupNum { get; set; }

        public int Role { get; set; } = 2;
        public DateTime Datatime { get; set; } = DateTime.Now;
        public string Nickname { get; set; }
    }
}
