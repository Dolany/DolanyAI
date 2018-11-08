using System.Linq;
using Dolany.Ai.Reborn.DolanyAI.Common;
using Dolany.Ai.Reborn.DolanyAI.Net;

namespace Dolany.Ai.Reborn.DolanyAI.Ai.SingleCommand.Fortune
{
    public class StarFortuneParser : HtmlParser
    {
        public string Content = string.Empty;

        protected override void Parse()
        {
            var root = document.DocumentNode;

            var query = SearchNodes(root,
                n => n.Name == "div" &&
                     n.ChildAttributes("class").Any(p => p.Value == "xz_cont"));
            if (query.IsNullOrEmpty())
            {
                return;
            }

            Content = query.First().InnerText;
        }
    }
}
