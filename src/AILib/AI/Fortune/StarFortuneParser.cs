﻿/*已迁移*/

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
        public string Content = string.Empty;

        protected override void Parse()
        {
            var root = document.DocumentNode;

            var query = SearchNodes(root,
                (n) => n.Name == "div"
                && n.ChildAttributes("class").Any((p) => p.Value == "xz_cont"));
            if (query.IsNullOrEmpty())
            {
                return;
            }

            Content = query.First().InnerText;
        }
    }
}