using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class ImageCacheModel
    {
        public string guid { get; set; }
        public string url { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}