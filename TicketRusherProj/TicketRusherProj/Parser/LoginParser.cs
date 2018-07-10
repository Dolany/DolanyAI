using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketRusherProj.Html;

namespace TicketRusherProj.Parser
{
    public class LoginParser : HtmlParser
    {
        protected override void Parse()
        {
            base.Parse();

            var root = document.DocumentNode;
        }
    }
}