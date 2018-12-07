namespace Dolany.Ai.Core.Cache
{
    using System;

    using Dolany.Ai.Core.Common;
    using Dolany.Ai.Core.Db;

    using static Dolany.Ai.Core.API.CodeApi;

    public class MsgSender
    {
        public static MsgSender Instance { get; } = new MsgSender();

        public void PushMsg(MsgCommand msg)
        {
            using (var db = new AIDatabase())
            {
                db.MsgCommand.Add(msg);

                db.SaveChanges();
            }
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
