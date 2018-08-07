using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;
using Dolany.Ice.Ai.DolanyAI.Db;
using System.Timers;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class MsgSender
    {
        private static MsgSender instance;

        private Timer timer = new Timer();
        private int MaxLength = 300;

        private MsgSender()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(TimerUp);

            timer.Start();
        }

        private void TimerUp(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            SendAllMsgs();
            timer.Start();
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
            if (string.IsNullOrEmpty(msg.Msg))
            {
                return;
            }

            if (msg.Guid.IsNullOrEmpty())
            {
                msg.Guid = Guid.NewGuid().ToString();
            }

            if (msg.Msg.Length > MaxLength)
            {
                PushMsg(new SendMsgDTO
                {
                    Aim = msg.Aim,
                    Type = msg.Type,
                    Msg = msg.Msg.Substring(0, MaxLength),
                    Guid = msg.Guid,
                    SerialNum = msg.SerialNum
                });

                PushMsg(new SendMsgDTO
                {
                    Aim = msg.Aim,
                    Type = msg.Type,
                    Msg = msg.Msg.Substring(MaxLength, msg.Msg.Length - MaxLength),
                    Guid = msg.Guid,
                    SerialNum = msg.SerialNum + 1
                });

                return;
            }

            using (AIDatabase db = new AIDatabase())
            {
                db.MsgSendCache.Add(new MsgSendCache
                {
                    Id = Guid.NewGuid().ToString(),
                    Aim = msg.Aim,
                    Type = msg.Type == MsgType.Group ? 0 : 1,
                    Msg = msg.Msg
                });
                db.SaveChanges();
            }
        }

        private void SendAllMsgs()
        {
            using (AIDatabase db = new AIDatabase())
            {
                var msgs = db.MsgSendCache.OrderBy(p => p.Guid).ThenBy(p => p.SerialNum);
                foreach (var msg in msgs)
                {
                    SendMsg(new SendMsgDTO
                    {
                        Aim = msg.Aim,
                        Msg = msg.Msg,
                        Type = msg.Type == 0 ? MsgType.Group : MsgType.Private
                    });
                }

                db.MsgSendCache.RemoveRange(msgs);
                db.SaveChanges();
            }
        }

        private void SendMsg(SendMsgDTO msg)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
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