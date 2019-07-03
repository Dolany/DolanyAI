using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Doremi.Model;

namespace Dolany.Ai.Doremi.Common
{
    public class PowerStateMgr
    {
        public readonly string[] Ais = {"Doremi", "DoreFun"};

        private readonly List<PowerStateRecord> StateRecords;

        public static PowerStateMgr Instance { get; } = new PowerStateMgr();

        private PowerStateMgr()
        {
            StateRecords = Ais.Select(PowerStateRecord.Get).ToList();
        }

        public bool CheckPower(string aiName)
        {
            return StateRecords.FirstOrDefault(p => p.AiName == aiName)?.IsPowerOn ?? false;
        }

        public void PowerOn(string aiName)
        {
            var record = StateRecords.FirstOrDefault(p => p.AiName == aiName);
            if (record == null)
            {
                return;
            }

            record.IsPowerOn = true;
            record.Update();
        }

        public void PowerOff(string aiName)
        {
            var record = StateRecords.FirstOrDefault(p => p.AiName == aiName);
            if (record == null)
            {
                return;
            }

            record.IsPowerOn = false;
            record.Update();
        }
    }
}
