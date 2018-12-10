namespace DolanyAiDesktop
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class UDPConnector
    {
        private const int Port = 11255;

        private const string IP = "127.0.0.1";

        private static UdpClient udpClient;

        public static void Send(string msg)
        {
            Task.Factory.StartNew(() => SendAsnc(msg));
        }

        private static void Init()
        {
            var ipAddress = IPAddress.Parse(IP);
            udpClient = new UdpClient(Port);
            udpClient.Client.Bind(new IPEndPoint(ipAddress, Port));
        }

        private static async Task SendAsnc(string msg)
        {
            if (udpClient == null)
            {
                Init();
            }

            var bytes = Encoding.UTF8.GetBytes(msg);
            await udpClient.SendAsync(bytes, bytes.Length);
        }
    }
}
