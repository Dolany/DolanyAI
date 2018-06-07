using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.AI.HourAlert
{
    public class AlertInfo
    {
        public string AlertContent { get; set; }
        public long FromGroup { get; set; }
        public long Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public int AimHour { get; set; }

        public static AlertInfo Parse(string msg)
        {
            if(string.IsNullOrEmpty(msg) || !msg.StartsWith("报时 "))
            {
                return null;
            }

            msg = msg.Replace("报时 ", "");
            string[] strs = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if(strs == null || strs.Length != 2)
            {
                return null;
            }

            int aimHour;
            if(!int.TryParse(strs[0], out aimHour))
            {
                return null;
            }

            AlertInfo info = new AlertInfo()
            {
                AlertContent = strs[1],
                AimHour = aimHour
            };

            if(!info.IsValid)
            {
                return null;
            }

            return info;
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(AlertContent) && AimHour >= 0 && AimHour <= 23;
            }
        }
    }
}
