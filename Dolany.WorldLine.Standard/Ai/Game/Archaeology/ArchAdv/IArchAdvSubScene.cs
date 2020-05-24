using Dolany.Ai.Common.Models;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology.ArchAdv
{
    public interface IArchAdvSubScene
    {
        public string ArchType { get; set; }

        public ArchaeologySubSceneModel Scene { get; set; }

        public int Level { get; set; }

        public MsgInformationEx MsgDTO { get; set; }

        public bool StartAdv();
    }
}
