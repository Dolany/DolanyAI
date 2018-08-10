using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketRusherProj.Entities
{
    public class ConfigEntity : EntityBase
    {
        [DataColumn]
        public string Name { get; set; }
    }
}