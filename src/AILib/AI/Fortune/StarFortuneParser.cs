using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Html;

namespace AILib
{
    public class StarFortuneParser : HtmlParser
    {
        protected override void Parse()
        {
            var root = document.DocumentNode;
        }
    }
}