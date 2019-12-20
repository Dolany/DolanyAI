using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.AITools
{
    public class WSClientCheckerTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval
            }
        };
        public override bool Enabled { get; set; } = true;
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
            var disabledAis = dic.Where(p => !p.Value).Select(p => p.Key).ToList();
            if (disabledAis.Count <= 0 || disabledAis.Count >= dic.Count)
            {
                return;
            }

            var availbleAi = dic.Where(p => p.Value).RandElement().Key;
            var msg = $"【警告！】{string.Join(",", disabledAis)} 失联！";
            MsgSender.PushMsg(0, Global.DeveloperNumber, msg, availbleAi);
        }
    }
}
