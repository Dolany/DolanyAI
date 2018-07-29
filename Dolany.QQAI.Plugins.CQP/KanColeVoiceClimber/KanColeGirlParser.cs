using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.DolanyAI;
using HtmlAgilityPack;

namespace KanColeVoiceClimber
{
    public class KanColeGirlParser : HtmlParser
    {
        protected override void Parse()
        {
            base.Parse();

            var root = document.DocumentNode;
        }
    }
}