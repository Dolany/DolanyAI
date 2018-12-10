namespace Dolany.Ai.Web
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class UDPConnector
    {
        private const int Port = 11255;

        private const string IP = "127.0.0.1";

        private static UdpClient udpClient;

        private static void Init()
        {
            var ipAddress = IPAddress.Parse(IP);
            udpClient = new UdpClient(Port);
            udpClient.Client.Bind(new IPEndPoint(ipAddress, Port));
        }

        public static void Listen(Action<string> CallBack)
        {
            if (udpClient == null)
            {
                Init();
            }

            Task.Factory.StartNew(
                async () =>
                    {
                        while (true)
                        {
                            var r = await Read();
                            if (!string.IsNullOrEmpty(r))
                            {
                                CallBack(r);
                            }

                            Thread.Sleep(500);
                        }
                    });
        }

        private static async Task<string> Read()
        {
            var result = await udpClient.ReceiveAsync();
            return result.Buffer != null ? Encoding.UTF8.GetString(result.Buffer) : string.Empty;
        }
    }
}
