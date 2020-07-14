using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.UtilityTool;
using Dolany.WorldLine.KindomStorm.Ai.KindomStorm;

namespace Dolany.WorldLine.KindomStorm.AiTools
{
    public class SoldierStateRefreshTool : IScheduleTool
    {
        protected override List<ScheduleDoModel> ModelList { get; set; } = new List<ScheduleDoModel>()
        {
            new ScheduleDoModel()
            {
                Interval = SchedulerTimer.HourlyInterval,
                IsImmediately = true
            }
        };

        protected override void ScheduleDo(SchedulerTimer timer)
        {
            if (!CheckTodayRefresh())
            {
                return;
            }

            Logger.Log("Start SoldierStateRefresh.");
            Task.Run(() =>
            {
                RebelProcess();
                BecomeStarve();
            });
        }

        private static void BecomeStarve()
        {
            var allCastles = KindomCastle.GetAll();
            foreach (var castle in allCastles)
            {
                var soldierGroups = SoldierGroup.Get(castle.QQNum);
                if (soldierGroups.IsNullOrEmpty())
                {
                    continue;
                }

                var becomeRebelGroups = soldierGroups.Where(g => g.State == SoldierState.Starving && g.LastFeedTime.AddDays(6) <= DateTime.Now);
                foreach (var rebelGroup in becomeRebelGroups)
                {
                    rebelGroup.Count /= 2;
                    if (rebelGroup.Count > 0)
                    {
                        rebelGroup.Rebel();
                    }
                    else
                    {
                        rebelGroup.Die();
                    }
                }

                var becomeStarvingGroups = soldierGroups.Where(g => g.State == SoldierState.Working && g.LastFeedTime.AddDays(3) <= DateTime.Now);
                foreach (var starvingGroup in becomeStarvingGroups)
                {
                    starvingGroup.Starve();
                }

                Logger.Log($"{castle.QQNum}'s Groups Refreshed.");
            }
        }

        private static void RebelProcess()
        {
            var overDateRebels = SoldierGroup.OverDateRebels(10).ToList();

            foreach (var rebel in overDateRebels)
            {
                rebel.Die();
            }

            Logger.Log($"{overDateRebels.Count} Rebel(s) Dead.");
        }

        private static bool CheckTodayRefresh()
        {
            var rec = GlobalVarRecord.Get("SoldierRefresh");
            if (string.IsNullOrEmpty(rec.Value) || !DateTime.TryParse(rec.Value, out var lastTime) || lastTime >= DateTime.Today)
            {
                return false;
            }

            rec.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            rec.Update();
            return true;
        }
    }
}
