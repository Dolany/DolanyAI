using System;
using WebSocket4Net;

namespace Dolany.Temp
{
    public class WSClient
    {
        private readonly WebSocket                    client;
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
            client                 =  new WebSocket(url);
            client.MessageReceived += OnMessageReceived;
            client.Closed          += OnClosed;
            
            Connect();
        }

        public void Connect()
        {
            // if (client.State == WebSocketState.Open)
            // {
            //     IsConnected = true;
            //     WSMgr.OnConnectStateChanged(BindAi, true);
            //     return;
            // }
            //
            client.Open();
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

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // var message = e.Message;
            // var model   = JsonConvert.DeserializeObject<QQEventModel>(message);
            //
            // MsgInvoke(BindAi, model);
            Console.WriteLine(e.Message);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            // IsConnected = false;
            // if (!IsReconnecting)
            // {
            //     WSMgr.OnConnectStateChanged(BindAi, false);
            // }
            Console.WriteLine("Closed.");
        }

        public void Send(string msg)
        {
            client.Send(msg);
        }
    }
}