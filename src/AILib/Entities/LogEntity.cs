using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib;

namespace AILib.Entities
{
    public class LogEntity : EntityBase
    {
        [DataColumn]
        public DateTime CreateTime { get; set; }

        public static void Log(string msg)
        {
            DbMgr.Insert(new LogEntity()
            {
                Id = Guid.NewGuid().ToString(),
                CreateTime = DateTime.Now,
                Content = msg
            });
        }
    }
}
