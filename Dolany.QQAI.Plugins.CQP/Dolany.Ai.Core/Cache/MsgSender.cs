using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Cache
{
    using System.Linq;
    using System.Timers;

    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;
    using Dolany.Ai.Core.DTO;
    using Dolany.Ai.Util;
    using static Dolany.Ai.Core.Common.Utility;
    using static Dolany.Ai.Core.API.CodeApi;

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

                if (string.IsNullOrEmpty(msg.Guid))
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
            using (var db = new AIDatabase())
            {
                switch (msg.Type)
                {
                    case MsgType.Group:
                        db.MsgCommand.Add(
                            new MsgCommand
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Command = AiCommand.SendGroup,
                                    Msg = msg.Msg,
                                    Time = DateTime.Now,
                                    ToGroup = msg.Aim,
                                    ToQQ = 0
                                });
                        break;

                    case MsgType.Private:
                        db.MsgCommand.Add(
                            new MsgCommand
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Command = AiCommand.SendPrivate,
                                    Msg = msg.Msg,
                                    Time = DateTime.Now,
                                    ToGroup = 0,
                                    ToQQ = msg.Aim
                            });
                        break;
                }

                db.SaveChanges();
            }
        }
    }
}
