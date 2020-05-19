using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.AITools
{
    public class CPUMonitorTool : IScheduleTool
    {
        public BindAiSvc BindAiSvc { get; set; }

        private readonly PerformanceCounter pcCpuLoad;

        public CPUMonitorTool()
        {
            pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total") {MachineName = "."};
            pcCpuLoad.NextValue();
        }

        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.MinutelyInterval * 30,
                IsImmediately = true
            }
        };
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            var curLoad = pcCpuLoad.NextValue();
            if (curLoad < 80)
            {
                return;
            }

            var ai = BindAiSvc.AiDic.Values.Where(p => p.IsConnected).RandElement();
            MsgSender.PushMsg(0, Global.DeveloperNumber, $"CPU 超过警戒值！({curLoad}%)", ai.Name);
        }
    }
}
