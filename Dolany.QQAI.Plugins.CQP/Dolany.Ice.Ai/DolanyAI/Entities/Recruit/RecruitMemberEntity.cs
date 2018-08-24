using System;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class RecruitMemberEntity : EntityBase
    {
        [DataColumn]
        public string RecruitId { get; set; }

        [DataColumn]
        public long MemberQQ { get; set; }

        [DataColumn]
        public DateTime JoinTime { get; set; }
    }
}