using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;
using System.Timers;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class MsgSender
    {
        private static MsgSender instance;

        private Queue<SendMsgDTO> MsgQueue = new Queue<SendMsgDTO>();

        private Timer timer = new Timer();

        private MsgSender()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(TimerUp);

            timer.Start();
        }

        private void TimerUp(object sender, ElapsedEventArgs e)
        {
            SendAllMsgs();
        }

        public static MsgSender Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MsgSender();
                }

                return instance;
            }
        }

        public void PushMsg(SendMsgDTO msg)
        {
            MsgQueue.Enqueue(msg);
        }

        public void PushMsg(IEnumerable<SendMsgDTO> msgs)
        {
            foreach (var m in msgs)
            {
                MsgQueue.Enqueue(m);
            }
        }

        private void SendAllMsgs()
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                while (MsgQueue.Count() > 0)
                {
                    var msg = MsgQueue.Dequeue();
                    switch (msg.Type)
                    {
                        case MsgType.Group:
                            api.SendGroupMessage(msg.Aim.ToString(), msg.Msg);
                            break;

                        case MsgType.Private:
                            api.SendPrivateMessage(msg.Aim.ToString(), msg.Msg);
                            break;
                    }
                }
            }
        }
    }
}