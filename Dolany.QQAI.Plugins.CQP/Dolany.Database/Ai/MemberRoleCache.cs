namespace Dolany.Database.Ai
{
    using System;

    public class MemberRoleCache
    {
        public long QQNum { get; set; }
        public long GroupNum { get; set; }

        /// <summary>
        /// 群员角色：0.群主，1.管理员，2.群员
        /// </summary>
        public int Role { get; set; } = 2;
        public string Nickname { get; set; }
    }
}
