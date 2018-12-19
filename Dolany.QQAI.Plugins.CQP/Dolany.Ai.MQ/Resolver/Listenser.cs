﻿namespace Dolany.Ai.MQ.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Timers;

    using Dolany.Ai.MQ.Db;
    using Dolany.Ai.MQ.MahuaApis;
    using Dolany.Ai.Util;

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
                    ResovleCommand(command);
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

        private IEnumerable<MsgCommand> CommandList()
        {
            using (var db = new AIDatabaseEntities())
            {
                var list = db.MsgCommand.Where(cmd => cmd.AiNum == Utility.SelfQQNum).ToList();

                db.MsgCommand.RemoveRange(list);
                db.SaveChanges();

                return list;
            }
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
            }
        }

        private void GetAuthCode(string relationId)
        {
            InfoSender.Send(AiInformation.CommandBack, Utility.GetAuthCode(), relationId);
        }

        private void Restart()
        {
            APIEx.Restart();
        }

        private void Praise(long qqNum, int count, string relationId)
        {
            for (var i = 0; i < count; i++)
            {
                APIEx.SendPraise(qqNum.ToString());
                Thread.Sleep(100);
            }

            InfoSender.Send(AiInformation.CommandBack, RelationId: relationId);
        }

        private void ReturnGroupMemberInfo(string groupNum, string relationId)
        {
            var info = APIEx.GetGroupMemberList(groupNum);
            if (string.IsNullOrEmpty(info))
            {
                return;
            }

            InfoSender.Send(AiInformation.CommandBack, info, relationId);
        }

        private void ReturnBackMusic(string musicId, string relationId)
        {
            var music = APIEx._163Music(musicId);
            if (string.IsNullOrEmpty(music))
            {
                return;
            }

            InfoSender.Send(AiInformation.CommandBack, music, relationId);
        }

        private void SendMsg(MsgCommand command)
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

                    default:
                        throw new Exception("Unexpected Case");
                }
            }
        }
    }
}
