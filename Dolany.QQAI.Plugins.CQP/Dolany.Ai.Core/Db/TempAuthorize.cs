using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Db
{
    public class TempAuthorize
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public long GroupNum { get; set; }
        public DateTime AuthDate { get; set; }
        public string AuthName { get; set; }
    }
}
