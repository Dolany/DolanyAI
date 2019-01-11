namespace Dolany.Database.Ai
{
    using System;

    public class TempAuthorize : BaseEntity
    {
        public long QQNum { get; set; }
        public long GroupNum { get; set; }
        public DateTime AuthDate { get; set; }
        public string AuthName { get; set; }
    }
}
