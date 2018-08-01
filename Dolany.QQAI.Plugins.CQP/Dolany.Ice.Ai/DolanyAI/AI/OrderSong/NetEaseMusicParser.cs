using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class NetEaseMusicParser : HtmlParser
    {
        public string SongId;

        protected override void Parse()
        {
            base.Parse();

            var root = document.DocumentNode;

            // TODO
        }
    }
}