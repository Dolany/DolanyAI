using System;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class RecruitEntity : EntityBase
    {
        [DataColumn]
        public long Creator { get; set; }

        [DataColumn]
        public DateTime CreateTime { get; set; }

        [DataColumn]
        public double DuringSpan { get; set; }

        [DataColumn]
        public DateTime FinishTime { get; set; }

        [DataColumn]
        public int TotalCount { get; set; } = 0;

        [DataColumn]
        public bool IsDuring { get; set; } = false;
    }
}