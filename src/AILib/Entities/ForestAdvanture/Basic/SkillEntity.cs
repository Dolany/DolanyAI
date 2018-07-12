using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class SkillEntity : EntityBase
    {
        [DataColumn]
        public string Name { get; set; }

        [DataColumn]
        public int LevelNeed { get; set; }

        [DataColumn]
        public string Descrption { get; set; }

        [DataColumn]
        public string CareerId { get; set; }

        [DataColumn]
        public int IsAutoLearn { get; set; }
    }
}