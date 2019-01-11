using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class KanColeGirlVoice : BaseEntity
    {
        public string Name { get; set; }
        public string VoiceUrl { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; }
    }
}
