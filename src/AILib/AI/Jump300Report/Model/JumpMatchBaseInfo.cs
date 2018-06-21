using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.AI.Jump300Report
{
    public class JumpMatchBaseInfo
    {
        public string MatchKind { get; set; }
        public int TotalKill { get; set; }
        public int TotalDie { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan DuringSpan { get; set; }
    }
}