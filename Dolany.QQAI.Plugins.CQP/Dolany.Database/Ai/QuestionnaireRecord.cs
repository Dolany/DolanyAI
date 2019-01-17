using System;

namespace Dolany.Database.Ai
{
    public class QuestionnaireRecord : BaseEntity
    {
        public string QNo { get; set; }

        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Content { get; set; }
    }
}
