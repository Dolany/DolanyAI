using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolany.Database.Sqlite.Model
{
    public class MagicExamCache
    {
        public DateTime EndTime { get; set; }

        public Dictionary<string, bool> Qustions { get; set; } = new Dictionary<string, bool>();

        public string BookName { get; set; }

        public bool IsPassed => Qustions.All(q => q.Value);
    }
}
