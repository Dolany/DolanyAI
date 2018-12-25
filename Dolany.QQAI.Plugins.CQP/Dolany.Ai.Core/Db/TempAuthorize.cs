namespace Dolany.Ai.Core.Db
{
    using System;

    public class TempAuthorize
    {
        public string Id { get; set; }
        public long QQNum { get; set; }
        public long GroupNum { get; set; }
        public DateTime AuthDate { get; set; }
        public string AuthName { get; set; }
    }
}
