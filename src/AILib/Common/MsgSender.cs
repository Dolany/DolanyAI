using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework;

namespace AILib
{
    public class MsgSender
    {
        private static MsgSender instance;

        private Queue<SendMsgDTO> MsgQueue = new Queue<SendMsgDTO>();

        private System.Timers.Timer timer = new System.Timers.Timer();

        private MsgSender()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp);

            timer.Start();
        }

        private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)
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
            lock(MsgQueue)
            {
                MsgQueue.Enqueue(msg);
            }
        }

        public void PushMsg(IEnumerable<SendMsgDTO> msgs)
        {
            lock (MsgQueue)
            {
                foreach(var m in msgs)
                {
                    MsgQueue.Enqueue(m);
                }
            }
        }

        private void SendAllMsgs()
        {
            lock(MsgQueue)
            {
                while (MsgQueue.Count() > 0)
                {
                    var msg = MsgQueue.Dequeue();
                    if(msg.Aim < 0)
                    {
                        Logger.Log($@"SendTo Negative Number:{msg.Aim}");
                        continue;
                    }
                    if(msg.Aim > 2147483647)
                    {
                        Logger.Log($@"SendTo Extra Big Number:{msg.Aim}");
                    }

                    switch (msg.Type)
                    {
                        case MsgType.Group:
                            CQ.SendGroupMessage(msg.Aim, msg.Msg);
                            break;
                        case MsgType.Private:
                            CQ.SendPrivateMessage(msg.Aim, msg.Msg);
                            break;
                    }
                }
            }
        }
    }
}
