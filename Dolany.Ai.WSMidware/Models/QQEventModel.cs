using System.Collections.Generic;

namespace Dolany.Ai.WSMidware.Models
{
    public class QQEventModel
    {
        public string Id { get; set; }

        public Dictionary<string, object> Result { get; set; }

        public string Error { get; set; }

        public string Event { get;set; }

        public QQEventParams Params { get; set; }
    }

    public class QQEventParams
    {
        public int Type { get; set; }

        public string Msgid { get; set; }

        public string Group { get; set; }

        public string Qq { get; set; }

        public string Content { get; set; }

        public string Operator { get; set; }

        public string Seq { get; set; }

        public string Amount { get; set; }

        public string Message { get; set; }

        public string Id { get; set; }
    }
}
