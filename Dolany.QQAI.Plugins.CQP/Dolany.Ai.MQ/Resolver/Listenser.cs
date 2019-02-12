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
            Task.Run(() =>
            {
                try
                {
                    ResovleCommand(command);
                }
                catch (Exception ex)
                {
                    MahuaModule.RuntimeLogger.Log(ex);
                }
            });
        }

        private void ResovleCommand(MsgCommand command)
        {
            switch (command.Command)
            {
                case AiCommand.SendGroup:
                case AiCommand.SendPrivate:
                    SendMsg(command);
                    break;
                case AiCommand.Get163Music:
                    ReturnBackMusic(command.Msg, command.Id);
                    break;
                case AiCommand.GetGroupMemberInfo:
                    ReturnGroupMemberInfo(command.Msg, command.Id);
                    break;
                case AiCommand.Praise:
                    Praise(command.ToQQ, int.Parse(command.Msg), command.Id);
                    break;
                case AiCommand.Restart:
                    Restart();
                    break;
                case AiCommand.GetAuthCode:
                    GetAuthCode(command.Id);
                    break;
                case AiCommand.GetGroups:
                    GetGroups(command.Id);
                    break;
                case AiCommand.SetSilence:
                    SetSilence(command.Id, command.ToGroup, command.ToQQ, int.Parse(command.Msg));
                    break;
            }
        }

        private static void SetSilence(string relationId, long GroupNum, long QQNum, int DuringTime)
        {
            APIEx.Ban(GroupNum.ToString(), QQNum.ToString(), DuringTime);

            InfoSender.Send(AiInformation.CommandBack, Utility.GetAuthCode(), relationId);
        }

        private static void GetGroups(string relationId)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                var model = api.GetGroups();

                InfoSender.Send(AiInformation.CommandBack, model, relationId);
            }
        }

        private static void GetAuthCode(string relationId)
        {
            InfoSender.Send(AiInformation.CommandBack, Utility.GetAuthCode(), relationId);
        }

        private static void Restart()
        {
            APIEx.Restart();
        }

        private static void Praise(long qqNum, int count, string relationId)
        {
            var qqnumstr = qqNum.ToString();
            for (var i = 0; i < count; i++)
            {
                APIEx.SendPraise(qqnumstr);
                Thread.Sleep(100);
            }

            InfoSender.Send(AiInformation.CommandBack, RelationId: relationId);
        }

        private static void ReturnGroupMemberInfo(string groupNum, string relationId)
        {
            var info = APIEx.GetGroupMemberList(groupNum);
            if (string.IsNullOrEmpty(info))
            {
                return;
            }

            InfoSender.Send(AiInformation.CommandBack, info, relationId);
        }

        private static void ReturnBackMusic(string musicId, string relationId)
        {
            var music = APIEx._163Music(musicId);
            if (string.IsNullOrEmpty(music))
            {
                return;
            }

            InfoSender.Send(AiInformation.CommandBack, music, relationId);
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
