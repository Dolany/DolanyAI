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
