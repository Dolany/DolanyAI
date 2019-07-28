using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Ai.SingleCommand.IceNews;
using Dolany.Ai.Core.Cache;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.AITools
{
    public class NewsUpdateTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval * 2
            }
        };
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            var record = GlobalVarRecord.Get("LastNews");
            if (!string.IsNullOrEmpty(record.Value))
            {
                var model = JsonConvert.DeserializeObject<NewsCacheModel>(record.Value);
                if (model.UpdateTime.Date == DateTime.Now.Date)
                {
                    return;
                }
            }
            NewsMgr.Instance.TryToRefresh();
        }
    }
}
