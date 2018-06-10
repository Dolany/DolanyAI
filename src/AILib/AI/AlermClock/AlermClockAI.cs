using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;

namespace AILib
{
    [AI(Name = "AlermClockAI", Description = "AI for Alerm Clock.", IsAvailable = false)]
    public class AlermClockAI : AIBase
    {
        private List<AlermClockEntity> ClockList = new List<AlermClockEntity>();

        public AlermClockAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {

        }

        [EnterCommand(Command = "设置闹钟", SourceType = MsgType.Group)]
        public void SetClock(GroupMsgDTO MsgDTO)
        {
            string[] strs = MsgDTO.msg.Split(new char[] { ' ' });
            if(strs == null || strs.Length < 2)
            {
                return;
            }

            (int hour, int minute)? time = GenTimeFromStr(strs[0]);
            if(time == null)
            {
                return;
            }

            if(string.IsNullOrEmpty(strs[1]))
            {
                return;
            }

            AlermClockEntity entity = new AlermClockEntity()
            {
                Id = new Guid().ToString(),
                AimHourt = time.Value.hour,
                AimMinute = time.Value.minute,
                Content = strs[1],
                Creator = MsgDTO.fromQQ,
                GroupNumber = MsgDTO.fromGroup,
                CreateTime = DateTime.Now
            };

            try
            {
                DbMgr.Add(entity);
                StartClock(entity);

                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "闹钟设定成功！"
                });
            }
            catch(Exception ex)
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "闹钟设定失败！"
                });
                Common.SendMsgToDeveloper(ex);
            }
        }

        private void StartClock(AlermClockEntity entity)
        {

        }

        [EnterCommand(Command = "我的闹钟", SourceType = MsgType.Group)]
        public void QueryClock(GroupMsgDTO MsgDTO)
        {

        }

        [EnterCommand(Command = "删除闹钟", SourceType = MsgType.Group)]
        public void DeleteClock(GroupMsgDTO MsgDTO)
        {

        }

        private (int hour, int minute)? GenTimeFromStr(string timeStr)
        {
            string[] strs = timeStr.Split(new char[] { ':', '：' });
            if(strs == null || strs.Length != 2)
            {
                return null;
            }

            int hour;
            if(!int.TryParse(strs[0], out hour))
            {
                return null;
            }

            int minute;
            if (!int.TryParse(strs[1], out minute))
            {
                return null;
            }

            return (hour, minute);
        }
    }
}
