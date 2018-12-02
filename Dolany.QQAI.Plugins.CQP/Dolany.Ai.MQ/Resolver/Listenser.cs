using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Dolany.Ai.MQ.Db;
using Newbe.Mahua;
using Dolany.Ai.Util;

namespace Dolany.Ai.MQ.Resolver
{
    using Dolany.Ai.MQ.MahuaApis;

    public class Listenser
    {
        private Timer Ltimer = new Timer();

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
                foreach(var command in commands)
                {
                    ResovleCommand(command);
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                Ltimer.Start();
            }
        }

        private List<MsgCommand> CommandList()
        {
            using (var db = new AIDatabaseEntities())
            {
                var list = db.MsgCommand.ToList();

                db.MsgCommand.RemoveRange(list);
                db.SaveChanges();

                return list;
            }
        }

        private void ResovleCommand(MsgCommand command)
        {
            switch(command.Command)
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
            }
        }

        private void ReturnGroupMemberInfo(string groupNum, string relationId)
        {
            var info = APIEx.GetGroupMemberList(groupNum);
            if (string.IsNullOrEmpty(info))
            {
                return;
            }

            using (var db = new AIDatabaseEntities())
            {
                db.MsgInformation.Add(
                    new MsgInformation
                        {
                            Id = Guid.NewGuid().ToString(),
                            FromGroup = 0,
                            FromQQ = 0,
                            Msg = info,
                            RelationId = relationId,
                            Time = DateTime.Now
                        });

                db.SaveChanges();
            }
        }

        private void ReturnBackMusic(string musicId, string relationId)
        {
            var music = APIEx._163Music(musicId);
            if (string.IsNullOrEmpty(music))
            {
                return;
            }

            using (var db = new AIDatabaseEntities())
            {
                db.MsgInformation.Add(
                    new MsgInformation
                        {
                            Id = Guid.NewGuid().ToString(),
                            FromGroup = 0,
                            FromQQ = 0,
                            Msg = music,
                            RelationId = relationId,
                            Time = DateTime.Now
                        });

                db.SaveChanges();
            }
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
