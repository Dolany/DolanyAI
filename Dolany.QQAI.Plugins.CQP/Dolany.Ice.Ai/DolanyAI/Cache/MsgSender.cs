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
    public class MsgSender : IDisposable
    {
        private static MsgSender instance;

        private Timer timer = new Timer();

        private int SendMsgMaxLength
        {
            get
            {
                var config = Utility.GetConfig(nameof(SendMsgMaxLength));
                if (config.IsNullOrEmpty())
                {
                    Utility.SetConfig(nameof(SendMsgMaxLength), "400");
                    return 400;
                }

                return int.Parse(config);
            }
        }

        private MsgSender()
        {
            timer = new Timer
            {
                Interval = 1000,
                AutoReset = false,
                Enabled = true
            };
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

            if (msg.Msg.Length > SendMsgMaxLength)
            {
                PushMsg(new SendMsgDTO
                {
                    Aim = msg.Aim,
                    Type = msg.Type,
                    Msg = msg.Msg.Substring(0, SendMsgMaxLength),
                    Guid = msg.Guid,
                    SerialNum = msg.SerialNum
                });

                PushMsg(new SendMsgDTO
                {
                    Aim = msg.Aim,
                    Type = msg.Type,
                    Msg = msg.Msg.Substring(SendMsgMaxLength, msg.Msg.Length - SendMsgMaxLength),
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

        private static void SendAllMsgs()
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

        private static void SendMsg(SendMsgDTO msg)
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

                    default:
                        throw new Exception("Unexpected Case");
                }
            }
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}