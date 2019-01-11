namespace Dolany.Ai.Core.Model
{
    using Common;
    using Dolany.Database.Ai;

    public class MsgInformationEx : MsgInformation
    {
        public string FullMsg { get; set; }
        public string Command { get; set; }
        public MsgType Type { get; set; }
        public string AuthName { get; set; }
    }
}
