using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Ai.SingleCommand.GroupManage;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.AITools
{
    public class AutoTimingTool: IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.NextHourInterval
            }
        };

        public override bool Enabled { get; set; } = true;
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            var curHour = DateTime.Now.Hour;
            var settings = MongoService<AutoPowerSetting>.Get(p => p.Hour == curHour);
            foreach (var setting in settings)
            {
                switch (setting.ActionType)
                {
                    case AutoPowerSettingActionType.PowerOn:
                    {
                        var groupSetting = GroupSettingMgr.Instance[setting.GroupNum];
                        groupSetting.IsPowerOn = true;
                        groupSetting.Update();
                        GroupSettingMgr.Instance.Refresh();

                        MsgSender.PushMsg(setting.GroupNum, 0, "开机成功！", groupSetting.BindAi);
                        break;
                    }
                    case AutoPowerSettingActionType.PowerOff:
                    {
                        var groupSetting = GroupSettingMgr.Instance[setting.GroupNum];
                        groupSetting.IsPowerOn = false;
                        groupSetting.Update();
                        GroupSettingMgr.Instance.Refresh();

                        MsgSender.PushMsg(setting.GroupNum, 0, "关机成功！", groupSetting.BindAi);
                        break;
                    }
                }
            }
            timer.Interval = SchedulerTimer.NextHourInterval;
        }
    }
}
