using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Html
{
    public class HtmlDocument
    {
        public HtmlElement Root { get; set; }

        public HtmlElement Head { get; set; }

        public HtmlDocument Body { get; set; }

        public void Parse(string htmlStr)
        {
        }
    }
}