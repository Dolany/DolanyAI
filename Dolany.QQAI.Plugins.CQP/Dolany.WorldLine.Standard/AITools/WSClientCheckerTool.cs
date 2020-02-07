using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Standard.AITools
{
    public class WSClientCheckerTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.MinutelyInterval
            }
        };
        public override bool Enabled { get; set; } = false;
        protected override void ScheduleDo(SchedulerTimer timer)
        {
            var command = new MsgCommand()
            {
                Command = CommandType.ConnectionState,
                BindAi = Global.DefaultConfig.MainAi
            };

            var info = Waiter.Instance.WaitForRelationId(command);
            if (info == null)
            {
                return;
            }

            var dic = JsonConvert.DeserializeObject<Dictionary<string, bool>>(info.Msg);
            foreach (var (bindaiName, state) in dic)
            {
                BindAiMgr.Instance[bindaiName].IsConnected = state;
            }
        }
    }
}
