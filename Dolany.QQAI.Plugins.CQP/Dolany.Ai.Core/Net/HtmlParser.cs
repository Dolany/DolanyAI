using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HtmlAgilityPack;

namespace Dolany.Ai.Core.Net
{
    public class HtmlParser
    {
        protected HtmlDocument document = new HtmlDocument();

        public void Load(string html)
        {
            document.LoadHtml(html);

            Parse();
        }

        protected virtual void Parse()
        {
        }

        protected List<HtmlNode> SearchNodes(HtmlNode root, Expression<Func<HtmlNode, bool>> express)
        {
            var result = new List<HtmlNode>();
            if (express.Compile()(root))
            {
                result.Add(root);
            }

            foreach (var child in root.ChildNodes)
            {
                result.AddRange(SearchNodes(child, express));
            }

            return result;
        }
    }
}
