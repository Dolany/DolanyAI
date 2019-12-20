using Dolany.Database;

namespace Dolany.Ai.Core.Ai.SingleCommand.GroupManage
{
    public class AutoPowerSetting : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public int Hour { get; set; }

        public AutoPowerSettingActionType ActionType { get; set; }
    }

    public enum AutoPowerSettingActionType
    {
        PowerOn = 0,
        PowerOff = 1
    }
}
