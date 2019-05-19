using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.AITools
{
    public class GroupInfoRefreshTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval
            }
        };
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            GroupSettingMgr.Instance.Refresh();
        }
    }
}
