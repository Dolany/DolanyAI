namespace Dolany.Ai.MQ.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;

    using Db;
    using MahuaApis;
    using Util;

    using Newbe.Mahua;

    using Timer = System.Timers.Timer;

    public class Listenser
    {
        private readonly Timer Ltimer = new Timer();

        public Listenser()
        {
            Ltimer.Elapsed += Timer_TimesUp;
            Ltimer.Interval = 500;
            Ltimer.AutoReset = false;

            Ltimer.Start();
        }

        private void Timer_TimesUp(object sender, ElapsedEventArgs e)
        {
            Ltimer.Stop();

            try
            {
                var commands = CommandList();
                foreach (var command in commands)
                {
                    Task.Run(() => ResovleCommand(command));
                }
            }
            catch (Exception ex)
            {
                MahuaModule.RuntimeLogger.Log(ex);
            }
            finally
            {
                Ltimer.Start();
            }
        }

        private static IEnumerable<MsgCommand> CommandList()
        {
            var list = MongoService<MsgCommand>.Get();

            MongoService<MsgCommand>.DeleteMany(list);

            return list;
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
            }
        }

        private static async Task GetAuthCode(string relationId)
        {
            await InfoSender.Send(AiInformation.CommandBack, Utility.GetAuthCode(), relationId);
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

            await InfoSender.Send(AiInformation.CommandBack, RelationId: relationId);
        }

        private static async Task ReturnGroupMemberInfo(string groupNum, string relationId)
        {
            var info = APIEx.GetGroupMemberList(groupNum);
            if (string.IsNullOrEmpty(info))
            {
                return;
            }

            await InfoSender.Send(AiInformation.CommandBack, info, relationId);
        }

        private static async Task ReturnBackMusic(string musicId, string relationId)
        {
            var music = APIEx._163Music(musicId);
            if (string.IsNullOrEmpty(music))
            {
                return;
            }

            await InfoSender.Send(AiInformation.CommandBack, music, relationId);
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
