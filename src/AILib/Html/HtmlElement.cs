using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AILib.Html
{
    public class HtmlElement
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public List<KeyValuePair<string, string>> Attributes { get; set; }
        public List<HtmlElement> Children { get; set; }

        public List<HtmlElement> GetChildren(string childName)
        {
            List<HtmlElement> list = new List<HtmlElement>();
            foreach (var c in Children)
            {
                if (c.Name == childName)
                {
                    list.Add(c);
                }
            }

            return list;
        }

        public HtmlElement GetChild(string childName)
        {
            foreach (var c in Children)
            {
                if (c.Name == childName)
                {
                    return c;
                }
            }

            return null;
        }

        public List<HtmlElement> GetChildren(Expression<Func<HtmlElement, bool>> express)
        {
            List<HtmlElement> list = new List<HtmlElement>();
            foreach (var c in Children)
            {
                if (express.Compile()(c))
                {
                    list.Add(c);
                }
            }

            return list;
        }
    }
}