using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Common;
using Dolany.UtilityTool;

namespace Dolany.Ai.Core.Cache
{
    public class MsgSender
    {
        public static GroupSettingSvc GroupSettingSvc => AutofacSvc.Resolve<GroupSettingSvc>();

        private static readonly ConcurrentDictionary<string, MsgPack> PackList = new ConcurrentDictionary<string, MsgPack>();

        public static void PushMsg(MsgCommand msg)
        {
            msg.Time = DateTime.Now;
            var callback =
                $"[{msg.BindAi}][Command] {(msg.ToGroup == 0 ? "私聊" : GroupSettingSvc[msg.ToGroup].Name)} {msg.ToQQ} {msg.Command} {msg.Msg}";
            Global.MsgPublish(callback);

            Global.CommandInfoService.Send(msg, Global.DefaultConfig.CommandQueueName);
            RestrictorSvc.Cache(msg.BindAi);
        }

        public static void PushMsg(MsgInformationEx MsgInfo, string Content, bool isNeedAt = false)
        {
            PushMsg(new MsgCommand
            {
                Command = MsgInfo.Type == MsgType.Group ? CommandType.SendGroup : CommandType.SendPrivate,
                Msg = MsgInfo.Type == MsgType.Group && isNeedAt ? $"{CodeApi.Code_At(MsgInfo.FromQQ)} {Content}" : Content,
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

        public static string StartSession(long ToGroup, long ToQQ, string BindAi)
        {
            var pack = new MsgPack()
            {
                ToGroup = ToGroup,
                ToQQ = ToQQ,
                BindAi = BindAi
            };

            var id = Guid.NewGuid().ToString();
            PackList.TryAdd(id, pack);

            return id;
        }

        public static string StartSession(MsgInformationEx MsgInfo)
        {
            return StartSession(MsgInfo.FromGroup, MsgInfo.FromQQ, MsgInfo.BindAi);
        }

        public static void PushMsg(string id, string content)
        {
            if (!PackList.ContainsKey(id))
            {
                return;
            }

            var pack = PackList[id];
            pack.ContentList.Add(content);
        }

        public static void ConfirmSend(string id)
        {
            if (!PackList.TryRemove(id, out var pack))
            {
                return;
            }

            PushMsg(pack.ToGroup, pack.ToQQ, pack.PackedContent, pack.BindAi);
        }
    }

    public class MsgPack
    {
        public long ToGroup { get; set; }

        public long ToQQ { get; set; }

        public string BindAi { get; set; }

        public List<string> ContentList { get; set; } = new List<string>();

        public string PackedContent => ContentList.JoinToString("\r\n");
    }
}
