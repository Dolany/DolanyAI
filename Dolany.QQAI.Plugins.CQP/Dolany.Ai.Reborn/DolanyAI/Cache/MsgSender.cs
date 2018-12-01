﻿using System;
using System.Linq;
using System.Timers;
using Dolany.Ai.Reborn.DolanyAI.Common;
using Dolany.Ai.Reborn.DolanyAI.Db;
using Dolany.Ai.Reborn.DolanyAI.DTO;
using Newbe.Mahua;
using static Dolany.Ai.Reborn.MahuaApis.CodeApi;
using static Dolany.Ai.Reborn.DolanyAI.Common.Utility;

namespace Dolany.Ai.Reborn.DolanyAI.Cache
{
    public class MsgSender
    {
        private int SendMsgMaxLength
        {
            get
            {
                var config = GetConfig(nameof(SendMsgMaxLength), "800");

                return int.Parse(config);
            }
        }

        private MsgSender()
        {
            JobScheduler.Instance.Add(1000, TimerUp);
        }

        private static void TimerUp(object sender, ElapsedEventArgs e)
        {
            SendAllMsgs();
        }

        public static MsgSender Instance { get; } = new MsgSender();

        public void PushMsg(ReceivedMsgDTO MsgDTO, string MsgContent, bool IsNeedAt = false)
        {
            var aim = MsgDTO.MsgType == MsgType.Group ? MsgDTO.FromGroup : MsgDTO.FromQQ;
            var msg = MsgDTO.MsgType == MsgType.Group && IsNeedAt
                ? $"{Code_At(MsgDTO.FromQQ)} {MsgContent}"
                : MsgContent;
            PushMsg(new SendMsgDTO
            {
                Aim = aim,
                Msg = msg,
                Type = MsgDTO.MsgType
            });
        }

        public void PushMsg(SendMsgDTO msg)
        {
            while (true)
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

                    msg = new SendMsgDTO
                    {
                        Aim = msg.Aim,
                        Type = msg.Type,
                        Msg = msg.Msg.Substring(SendMsgMaxLength, msg.Msg.Length - SendMsgMaxLength),
                        Guid = msg.Guid,
                        SerialNum = msg.SerialNum + 1
                    };
                    continue;
                }

                using (var db = new AIDatabase())
                {
                    db.MsgSendCache.Add(new MsgSendCache
                    {
                        Id = Guid.NewGuid().ToString(),
                        Aim = msg.Aim,
                        Type = msg.Type == MsgType.Group ? 0 : 1,
                        Msg = msg.Msg,
                        AINum = SelfQQNum,
                        Guid = msg.Guid,
                        SerialNum = msg.SerialNum
                    });
                    db.SaveChanges();
                }

                break;
            }
        }

        private static void SendAllMsgs()
        {
            using (var db = new AIDatabase())
            {
                var aiNum = SelfQQNum;
                var msgs = db.MsgSendCache.Where(p => p.AINum == aiNum)
                                          .OrderBy(p => p.Guid)
                                          .ThenBy(p => p.SerialNum);
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
    }
}