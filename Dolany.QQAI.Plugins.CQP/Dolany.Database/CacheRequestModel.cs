namespace Dolany.Database
{
    using System;

    public class CacheRequestModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 读取缓存返回队列名称（请求类型为1时，为空）
        /// </summary>
        public string ResponseQueueName { get; set; }

        public string RequestTable { get; set; }

        public string RequestKey { get; set; }

        /// <summary>
        /// 存入缓存的值（请求类型为2时，为空）
        /// </summary>
        public string RequestValue { get; set; }

        /// <summary>
        /// 过期时间（请求类型为2时，为空）
        /// </summary>
        public DateTime? ExpireTime { get; set; } = DateTime.Now.AddDays(1);

        /// <summary>
        /// 请求类型：1.存入缓存；2.读取缓存
        /// </summary>
        public int RequestType { get; set; }
    }
}
