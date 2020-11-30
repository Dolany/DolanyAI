using System;
using System.Threading.Tasks;
using WebSocketExtensions;

namespace Dolany.Temp
{
    public class WSClient
    {
        private readonly WebSocketClient client;

        private readonly string url;
        // private readonly Action<string, QQEventModel> MsgInvoke;

        // public WSClient(string url, string BindAi, Action<string, QQEventModel> MsgInvoke)
        // {
        //     this.BindAi            =  BindAi;
        //     this.MsgInvoke         =  MsgInvoke;
        //     client                 =  new WebSocket(url);
        //     client.MessageReceived += OnMessageReceived;
        //     client.Closed          += OnClosed;
        //
        //     Connect();
        // }
        public WSClient(string url)
        {
            client   = new WebSocketClient {MessageHandler = OnMessageReceived, CloseHandler = OnClosed};
            this.url = url;
        }

        public async Task Connect()
        {
            // if (client.State == WebSocketState.Open)
            // {
            //     IsConnected = true;
            //     WSMgr.OnConnectStateChanged(BindAi, true);
            //     return;
            // }
            //
            await client.ConnectAsync(url);
            //
            // Thread.Sleep(1000);
            // if (client.State != WebSocketState.Open)
            // {
            //     return;
            // }
            //
            // IsConnected = true;
            // WSMgr.OnConnectStateChanged(BindAi, true);
            Console.WriteLine("Connet.");
        }

        private void OnMessageReceived(StringMessageReceivedEventArgs e)
        {
            // var message = e.Message;
            // var model   = JsonConvert.DeserializeObject<QQEventModel>(message);
            //
            // MsgInvoke(BindAi, model);
            Console.WriteLine(e.Data);
        }

        private void OnClosed(WebSocketReceivedResultEventArgs e)
        {
            // IsConnected = false;
            // if (!IsReconnecting)
            // {
            //     WSMgr.OnConnectStateChanged(BindAi, false);
            // }
            Console.WriteLine("Closed.");
        }

        public async Task Send(string msg)
        {
            await client.SendStringAsync(msg);
        }
    }
}