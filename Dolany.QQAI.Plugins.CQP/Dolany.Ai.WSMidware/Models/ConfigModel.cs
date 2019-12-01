namespace Dolany.Ai.WSMidware.Models
{
    public class ConfigModel
    {
        public string[] AiList { get; set; }

        public int ReconnectSecords { get; set; }

        public string MQReceiveQueue { get; set; }

        public string MQSendQueue { get; set; }
    }
}
