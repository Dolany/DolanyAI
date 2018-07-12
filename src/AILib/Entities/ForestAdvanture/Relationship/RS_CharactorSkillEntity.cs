using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class RS_CharactorSkillEntity : EntityBase
    {
        [DataColumn]
        public string CharId { get; set; }

        [DataColumn]
        public string SkillId { get; set; }
    }
}