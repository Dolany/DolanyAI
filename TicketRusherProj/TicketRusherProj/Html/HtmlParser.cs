using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq.Expressions;

namespace TicketRusherProj.Html
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
            List<HtmlNode> result = new List<HtmlNode>();
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