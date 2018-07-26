﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class MsgSender
    {
        private static MsgSender instance;

        //private Queue<SendMsgDTO> MsgQueue = new Queue<SendMsgDTO>();

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
            using (AIDatabase db = new AIDatabase())
            {
                db.MsgSendCache.Add(new MsgSendCache
                {
                    Id = Guid.NewGuid().ToString(),
                    Aim = msg.Aim,
                    Type = msg.Type == MsgType.Group ? 0 : 1
                });
                db.SaveChanges();
            }
        }

        private void SendAllMsgs()
        {
            using (AIDatabase db = new AIDatabase())
            {
                var msgs = db.MsgSendCache;
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