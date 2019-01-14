namespace Dolany.Cache
{
    using System;

    public class CacheModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? ExpireTime { get; set; }
    }
}
