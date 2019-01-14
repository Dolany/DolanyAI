namespace Dolany.Ai.Core.Cache
{
    using System;

    using Common;

    using Database;

    using Dolany.Ai.Common;
    using Dolany.Database.Ai;

    using Model;

    using static API.CodeApi;

    public class MsgSender
    {
        public static MsgSender Instance { get; } = new MsgSender();

        public void PushMsg(MsgCommand msg)
        {
            msg.AiNum = Utility.SelfQQNum;
            msg.Time = DateTime.Now;
            var callback = $"[Command] {msg.ToGroup} {msg.ToQQ} {msg.Id} {msg.Command} {msg.Msg}";
            AIMgr.Instance.MessagePublish(callback);

            Global.CommandInfoService.Send(msg, CommonUtil.GetConfig("MsgInformationName"));
        }

        public void PushMsg(MsgInformationEx MsgInfo, string Content, bool isNeedAt = false)
        {
            PushMsg(
                new MsgCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        Command = MsgInfo.Type == MsgType.Group ? AiCommand.SendGroup : AiCommand.SendPrivate,
                        Msg = MsgInfo.Type == MsgType.Group && isNeedAt
                                  ? $"{Code_At(MsgInfo.FromQQ)} {Content}"
                                  : Content,
                        Time = DateTime.Now,
                        ToGroup = MsgInfo.FromGroup,
                        ToQQ = MsgInfo.FromQQ
                    });
        }
    }
}
