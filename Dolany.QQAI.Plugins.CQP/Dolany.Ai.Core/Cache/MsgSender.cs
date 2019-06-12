﻿using System;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.Cache
{
    public class MsgSender
    {
        public static void PushMsg(MsgCommand msg)
        {
            msg.Time = DateTime.Now;
            var callback = $"[Command] {(msg.ToGroup == 0 ? "私聊" : GroupSettingMgr.Instance[msg.ToGroup].Name)} {msg.ToQQ} {msg.Id} {msg.Command} {msg.Msg}";
            AIMgr.Instance.MessagePublish(callback);

            if (string.IsNullOrEmpty(msg.BindAi))
            {
                RuntimeLogger.Log("Null BindAi:" + JsonConvert.SerializeObject(msg));
                return;
            }

            Global.CommandInfoService.Send(msg, BindAiMgr.Instance[msg.BindAi].CommandQueue);
        }

        public static void PushMsg(MsgInformationEx MsgInfo, string Content, bool isNeedAt = false)
        {
            PushMsg(
                new MsgCommand
                    {
                        Command = MsgInfo.Type == MsgType.Group ? CommandType.SendGroup : CommandType.SendPrivate,
                        Msg = MsgInfo.Type == MsgType.Group && isNeedAt
                                  ? $"{CodeApi.Code_At(MsgInfo.FromQQ)} {Content}"
                                  : Content,
                        ToGroup = MsgInfo.FromGroup,
                        ToQQ = MsgInfo.FromQQ,
                        BindAi = MsgInfo.BindAi
                    });
        }

        public static void PushMsg(long GroupNum, long QQNum, string content, string BindAi)
        {
            PushMsg(new MsgInformationEx
            {
                FromGroup = GroupNum,
                FromQQ = QQNum,
                Type = GroupNum == 0 ? MsgType.Private : MsgType.Group,
                BindAi = BindAi
            }, content, QQNum != 0);
        }
    }
}
