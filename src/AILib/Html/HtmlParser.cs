using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AILib.Html
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
    }
}