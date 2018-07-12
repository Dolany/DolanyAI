using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class HeroCharactorEntity : EntityBase
    {
        [DataColumn]
        public string HeroName { get; set; }

        [DataColumn]
        public int Level { get; set; }

        [DataColumn]
        public int ExpForNextLevel { get; set; }

        [DataColumn]
        public int AttackBase { get; set; }

        [DataColumn]
        public int MagicBase { get; set; }

        [DataColumn]
        public int HPFull { get; set; }

        [DataColumn]
        public int MPFull { get; set; }

        [DataColumn]
        public int HPCurrent { get; set; }

        [DataColumn]
        public int MPCurrent { get; set; }

        [DataColumn]
        public int Courage { get; set; }
    }
}