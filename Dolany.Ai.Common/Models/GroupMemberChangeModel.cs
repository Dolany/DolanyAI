namespace Dolany.Ai.Common.Models
{
    public class GroupMemberChangeModel
    {
        public long GroupNum { get; set; }

        public long QQNum { get;set; }

        /// <summary>
        /// 改变类型：0为加群，1为退群
        /// </summary>
        public int Type { get; set; }

        public long Operator { get; set; }
    }
}
