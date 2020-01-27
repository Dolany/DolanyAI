using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Common.PicReview
{
    public class PicReviewRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public long GroupNum { get; set; }

        public string Usage { get; set; }

        public PicReviewStatus Status { get; set; } = PicReviewStatus.Waiting;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ReviewTime { get; set; }

        public string PicName { get; set; }

        public void Update()
        {
            MongoService<PicReviewRecord>.Update(this);
        }

        public void Insert()
        {
            MongoService<PicReviewRecord>.Insert(this);
        }
    }

    public enum PicReviewStatus
    {
        Waiting,
        Passed,
        Refused
    }
}
