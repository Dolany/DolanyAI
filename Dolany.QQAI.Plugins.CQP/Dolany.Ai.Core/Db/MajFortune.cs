namespace Dolany.Ai.Core.Db
{
    using System;

    public class MajFortune
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int FortuneStar { get; set; }
        public string Position { get; set; }
        public string Kind { get; set; }
        public string CharactorName { get; set; }
        public string CharactorPath { get; set; }
        public long QQNum { get; set; }
        public DateTime UpdateTime { get; set; } = DateTime.Now.Date;
    }
}
