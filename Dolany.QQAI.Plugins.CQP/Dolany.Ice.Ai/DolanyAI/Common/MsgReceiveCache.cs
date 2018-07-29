﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class MsgReceiveCache
    {
        private Timer timer = new Timer();

        //private Queue<GroupMsgDTO> GroupMsgQueue = new Queue<GroupMsgDTO>();

        private Action<GroupMsgDTO> CallBack = null;

        public void PushMsg(GroupMsgDTO MsgDTO)
        {
            using (AIDatabase db = new AIDatabase())
            {
                MsgRecievedCache cache = new MsgRecievedCache
                {
                    Id = Guid.NewGuid().ToString(),
                    FromGroup = MsgDTO.FromGroup,
                    FromQQ = MsgDTO.FromQQ,
                    Msg = MsgDTO.Msg,
                    FullMsg = MsgDTO.FullMsg,
                    Command = MsgDTO.Command,
                    Time = DateTime.Now
                };

                db.MsgRecievedCache.Add(cache);
                db.SaveChanges();
            }
        }

        public MsgReceiveCache(Action<GroupMsgDTO> CallBack)
        {
            this.CallBack = CallBack;

            timer.Interval = 500;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += TimeUp;
            timer.Start();
        }

        private void TimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var msgs = db.MsgRecievedCache;
                foreach (var msg in msgs)
                {
                    CallBack(new GroupMsgDTO
                    {
                        Msg = msg.Msg,
                        Command = msg.Command,
                        FullMsg = msg.FullMsg,
                        FromGroup = msg.FromGroup,
                        FromQQ = msg.FromQQ,
                        //SendTime = msg.Time.to
                    });
                }

                db.MsgRecievedCache.RemoveRange(msgs);
                db.SaveChanges();
            }
        }
    }
}