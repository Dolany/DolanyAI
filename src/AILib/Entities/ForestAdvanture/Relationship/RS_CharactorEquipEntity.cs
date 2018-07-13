using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class RS_CharactorEquipEntity : EntityBase
    {
        [DataColumn]
        public string CharactorId { get; set; }

        [DataColumn]
        public string EquipId { get; set; }
    }
}