using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class CharactorSettingEntity : EntityBase
    {
        [DataColumn]
        public long GroupNumber { get; set; }

        [DataColumn]
        public long Creator { get; set; }

        [DataColumn]
        public DateTime CreateTime { get; set; }

        [DataColumn]
        public string Charactor { get; set; }

        [DataColumn]
        public string SettingName { get; set; }
    }
}