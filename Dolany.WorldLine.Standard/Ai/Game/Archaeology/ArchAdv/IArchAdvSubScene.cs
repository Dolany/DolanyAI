using System.Collections.Generic;
using Dolany.Ai.Common.Models;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology.ArchAdv
{
    public interface IArchAdvSubScene
    {
        public string ArchType { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public MsgInformationEx MsgDTO { get; set; }

        public bool StartAdv();
    }
}
