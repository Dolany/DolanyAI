using System.Collections.Generic;
using Dolany.Ai.Common.Models;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology.ArchAdv
{
    public class ArchAdvSubScene_Monster : IArchAdvSubScene
    {
        public string ArchType { get; set; } = "Monster";
        public Dictionary<string, object> Data { get; set; }
        public MsgInformationEx MsgDTO { get; set; }
        public bool StartAdv()
        {
            // todo
            return default;
        }
    }
}
