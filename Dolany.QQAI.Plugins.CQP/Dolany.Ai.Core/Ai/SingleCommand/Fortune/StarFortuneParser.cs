using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Net;

namespace Dolany.Ai.Core.Ai.SingleCommand.Fortune
{
    public class StarFortuneParser : HtmlParser
    {
        public string Content = string.Empty;

        private readonly string[] Dims = {"综合运势:", "爱情运:","工作运:","财运:"};

        protected override void Parse()
        {
            var root = document.DocumentNode;

            var query = SearchNodes(root, n => n.Name == "div" && n.ChildAttributes("class").Any(p => p.Value == "xz_cont"));
            if (query.IsNullOrEmpty())
            {
                return;
            }

            var node = query.First();
            var imgs = SearchNodes(node, n => n.Name == "img");
            if (imgs.IsNullOrEmpty() || imgs.Count < 4)
            {
                return;
            }

            var msg = node.InnerText;
            var srcs = imgs.Select(i => CodeApi.Code_Image(i.Attributes.First(a => a.Name == "src").Value) + "\r").ToList();
            for (var i = 0; i < Dims.Length; i++)
            {
                var index = msg.IndexOf(Dims[i], StringComparison.Ordinal);
                index += Dims[i].Length;
                msg = msg.Insert(index, srcs[i]);
            }

            Content = msg;
        }
    }
}
