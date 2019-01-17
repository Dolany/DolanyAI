namespace Dolany.Ai.MQ.Resolver
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Db;
    using MahuaApis;
    using Util;

    using Newbe.Mahua;

    public class Listenser
    {
        public static Listenser Instance { get; } = new Listenser();

        private Listenser()
        {
        }

        public void ReceivedCommand(MsgCommand command)
        {
            try
            {
                Task.Run(() => ResovleCommand(command));
            }
            catch (Exception ex)
            {
                MahuaModule.RuntimeLogger.Log(ex);
            }
        }

        private static async Task ResovleCommand(MsgCommand command)
        {
            switch (command.Command)
            {
                case AiCommand.SendGroup:
                case AiCommand.SendPrivate:
                    SendMsg(command);
                    break;
                case AiCommand.Get163Music:
                    await ReturnBackMusic(command.Msg, command.Id);
                    break;
                case AiCommand.GetGroupMemberInfo:
                    await ReturnGroupMemberInfo(command.Msg, command.Id);
                    break;
                case AiCommand.Praise:
                    await Praise(command.ToQQ, int.Parse(command.Msg), command.Id);
                    break;
                case AiCommand.Restart:
                    Restart();
                    break;
                case AiCommand.GetAuthCode:
                    await GetAuthCode(command.Id);
                    break;
                case AiCommand.GetGroups:
                    await GetGroups(command.Id);
                    break;
            }
        }

        private static async Task GetGroups(string relationId)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                var model = api.GetGroups();

                await InfoSender.SendAsync(AiInformation.CommandBack, model, relationId);
            }
        }

        private static async Task GetAuthCode(string relationId)
        {
            await InfoSender.SendAsync(AiInformation.CommandBack, Utility.GetAuthCode(), relationId);
        }

        private static void Restart()
        {
            APIEx.Restart();
        }

        private static async Task Praise(long qqNum, int count, string relationId)
        {
            for (var i = 0; i < count; i++)
            {
                APIEx.SendPraise(qqNum.ToString());
                Thread.Sleep(100);
            }

            await InfoSender.SendAsync(AiInformation.CommandBack, RelationId: relationId);
        }

        private static async Task ReturnGroupMemberInfo(string groupNum, string relationId)
        {
            var info = APIEx.GetGroupMemberList(groupNum);
            if (string.IsNullOrEmpty(info))
            {
                return;
            }

            await InfoSender.SendAsync(AiInformation.CommandBack, info, relationId);
        }

        private static async Task ReturnBackMusic(string musicId, string relationId)
        {
            var music = APIEx._163Music(musicId);
            if (string.IsNullOrEmpty(music))
            {
                return;
            }

            await InfoSender.SendAsync(AiInformation.CommandBack, music, relationId);
        }

        private static void SendMsg(MsgCommand command)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                switch (command.Command)
                {
                    case AiCommand.SendGroup:
                        api.SendGroupMessage(command.ToGroup.ToString(), command.Msg);
                        break;

                    case AiCommand.SendPrivate:
                        api.SendPrivateMessage(command.ToQQ.ToString(), command.Msg);
                        break;
                }
            }
        }
    }
}
