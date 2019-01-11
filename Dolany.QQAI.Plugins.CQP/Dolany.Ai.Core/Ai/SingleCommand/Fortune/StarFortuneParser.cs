﻿using System.Linq;

namespace Dolany.Ai.Core.Ai.SingleCommand.Fortune
{
    using Common;
    using Net;

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
