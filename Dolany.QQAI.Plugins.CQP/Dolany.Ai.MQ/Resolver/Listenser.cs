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

        private static readonly object Lock = new object();

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
                case CommandType.SendGroup:
                case CommandType.SendPrivate:
                    SendMsg(command);
                    break;
                case CommandType.Get163Music:
                    ReturnBackMusic(command.Msg, command.Id);
                    break;
                case CommandType.GetGroupMemberInfo:
                    ReturnGroupMemberInfo(command.Msg, command.Id);
                    break;
                case CommandType.Praise:
                    Praise(command.ToQQ, int.Parse(command.Msg), command.Id);
                    break;
                case CommandType.Restart:
                    Restart();
                    break;
                case CommandType.GetAuthCode:
                    GetAuthCode(command.Id);
                    break;
                case CommandType.GetGroups:
                    GetGroups(command.Id);
                    break;
                case CommandType.SetSilence:
                    SetSilence(command.Id, command.ToGroup, command.ToQQ, int.Parse(command.Msg));
                    break;
            }
        }

        private static void SetSilence(string relationId, long GroupNum, long QQNum, int DuringTime)
        {
            APIEx.Ban(GroupNum.ToString(), QQNum.ToString(), DuringTime);

            InfoSender.Send(InformationType.CommandBack, Utility.GetAuthCode(), relationId);
        }

        private static void GetGroups(string relationId)
        {
            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                var model = api.GetGroups();

                InfoSender.Send(InformationType.CommandBack, model, relationId);
            }
        }

        private static void GetAuthCode(string relationId)
        {
            InfoSender.Send(InformationType.CommandBack, Utility.GetAuthCode(), relationId);
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

            InfoSender.Send(InformationType.CommandBack, RelationId: relationId);
        }

        private static void ReturnGroupMemberInfo(string groupNum, string relationId)
        {
            var info = APIEx.GetGroupMemberList(groupNum);
            if (string.IsNullOrEmpty(info))
            {
                return;
            }

            InfoSender.Send(InformationType.CommandBack, info, relationId);
        }

        private static void ReturnBackMusic(string musicId, string relationId)
        {
            var music = APIEx._163Music(musicId);
            if (string.IsNullOrEmpty(music))
            {
                return;
            }

            InfoSender.Send(InformationType.CommandBack, music, relationId);
        }

        private static void SendMsg(MsgCommand command)
        {
            lock (Lock)
            {
                using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                {
                    var api = robotSession.MahuaApi;
                    switch (command.Command)
                    {
                        case CommandType.SendGroup:
                            api.SendGroupMessage(command.ToGroup.ToString(), command.Msg);
                            break;

                        case CommandType.SendPrivate:
                            api.SendPrivateMessage(command.ToQQ.ToString(), command.Msg);
                            break;
                    }
                }
            }
        }
    }
}
