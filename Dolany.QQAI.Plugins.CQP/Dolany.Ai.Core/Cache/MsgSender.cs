namespace Dolany.Ai.Core.Cache
{
    using System;

    using Common;
    using Dolany.Ai.Common;
    using Model;

    using static API.CodeApi;

    public class MsgSender
    {
        public static MsgSender Instance { get; } = new MsgSender();

        public void PushMsg(MsgCommand msg)
        {
            msg.AiNum = Utility.SelfQQNum;
            msg.Time = DateTime.Now;
            var callback = $"[Command] {(msg.ToGroup == 0 ? "私聊" : GroupSettingMgr.Instance[msg.ToGroup].Name)} {msg.ToQQ} {msg.Id} {msg.Command} {msg.Msg}";
            AIMgr.Instance.MessagePublish(callback);

            Global.CommandInfoService.Send(msg, Configger.Instance["CommandQueueName"]);
        }

        public void PushMsg(MsgInformationEx MsgInfo, string Content, bool isNeedAt = false)
        {
            PushMsg(
                new MsgCommand
                    {
                        Command = MsgInfo.Type == MsgType.Group ? AiCommand.SendGroup : AiCommand.SendPrivate,
                        Msg = MsgInfo.Type == MsgType.Group && isNeedAt
                                  ? $"{Code_At(MsgInfo.FromQQ)} {Content}"
                                  : Content,
                        ToGroup = MsgInfo.FromGroup,
                        ToQQ = MsgInfo.FromQQ
                    });
        }

        public void PushMsg(long GroupNum, long QQNum, string content, bool isNeedAt = false)
        {
            PushMsg(new MsgInformationEx
            {
                FromGroup = GroupNum,
                FromQQ = QQNum,
                Type = GroupNum == 0 ? MsgType.Private : MsgType.Group
            }, content, isNeedAt);
        }
    }
}
