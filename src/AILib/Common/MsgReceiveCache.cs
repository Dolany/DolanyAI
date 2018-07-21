/*已迁移*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AILib
{
    public class MsgReceiveCache
    {
        private Timer timer = new Timer();

        private Queue<GroupMsgDTO> GroupMsgQueue = new Queue<GroupMsgDTO>();

        private Action<GroupMsgDTO> CallBack = null;

        public void PushMsg(GroupMsgDTO MsgDTO)
        {
            lock (GroupMsgQueue)
            {
                GroupMsgQueue.Enqueue(MsgDTO);
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
            lock (GroupMsgQueue)
            {
                while (GroupMsgQueue.Count > 0)
                {
                    var MsgDTO = GroupMsgQueue.Dequeue();
                    CallBack(MsgDTO);
                }
            }
        }
    }
}