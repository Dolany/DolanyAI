using System;
using System.Threading;
using Dolany.Ai.WSMidware.Models;
using Newtonsoft.Json;
using WebSocket4Net;

namespace Dolany.Ai.WSMidware
{
    public class WSClient
    {
        private readonly WebSocket client;
        private readonly Action<string, QQEventModel> MsgInvoke;

        public bool IsConnected;
        public readonly string BindAi;

        public WSClient(string url, string BindAi, Action<string, QQEventModel> MsgInvoke)
        {
            this.BindAi = BindAi;
            this.MsgInvoke = MsgInvoke;
            client = new WebSocket(url);
            client.MessageReceived += OnMessageReceived;
            client.Closed += OnClosed;

            Connect();
        }

        public void Connect()
        {
            client.Open();

            Thread.Sleep(1000);
            if (client.State != WebSocketState.Open)
            {
                return;
            }

            IsConnected = true;
            WSMgr.OnConnectStateChanged(BindAi, true);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var message = e.Message;
            var model = JsonConvert.DeserializeObject<QQEventModel>(message);

            MsgInvoke(BindAi, model);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            IsConnected = false;
            WSMgr.OnConnectStateChanged(BindAi, false);
        }

        public void Send(string msg)
        {
            client.Send(msg);
        }
    }
}
