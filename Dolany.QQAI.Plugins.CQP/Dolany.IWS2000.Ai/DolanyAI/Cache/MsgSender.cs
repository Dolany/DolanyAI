﻿using System;
using System.Linq;
using System.Timers;
using Newbe.Mahua;
using Dolany.IWS2000.Ai.DolanyAI.Db;

namespace Dolany.IWS2000.Ai.DolanyAI
{
    public class MsgSender
    {
        private static MsgSender instance;

        private int SendMsgMaxLength
        {
            get
            {
                var config = Utility.GetConfig(nameof(SendMsgMaxLength), "800");

                return int.Parse(config);
            }
        }

        private MsgSender()
        {
            JobScheduler.Instance.Add(1000, TimerUp);
        }

        private void TimerUp(object sender, ElapsedEventArgs e)
        {
            SendAllMsgs();
        }

        public static MsgSender Instance => instance ?? (instance = new MsgSender());

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

                // ReSharper disable once RedundantJumpStatement
                return;
            }

            using (var db = new AIDatabase())
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
            using (var db = new AIDatabase())
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

        // ReSharper disable once UnusedMember.Local
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
    }
}