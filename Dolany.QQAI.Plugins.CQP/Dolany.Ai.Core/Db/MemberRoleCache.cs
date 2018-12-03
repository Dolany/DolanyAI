using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public partial class MemberRoleCache
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public long GroupNum { get; set; }
        public int Role { get; set; }
        public System.DateTime Datatime { get; set; }
        public string Nickname { get; set; }
    }
}
