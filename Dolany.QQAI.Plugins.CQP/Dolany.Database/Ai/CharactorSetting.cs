using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Database.Ai
{
    public partial class CharactorSetting
    {
        public string Id { get; set; }
        public long GroupNumber { get; set; }
        public long Creator { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string Charactor { get; set; }
        public string SettingName { get; set; }
        public string Content { get; set; }
    }
}
