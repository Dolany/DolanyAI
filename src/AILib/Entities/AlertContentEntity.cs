using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    public class AlertContentEntity : EntityBase
    {
        [DataColumn]
        public long FromGroup { get; set; }
        [DataColumn]
        public long Creator { get; set; }
        [DataColumn]
        public DateTime CreateTime { get; set; }
        [DataColumn]
        public int AimHour { get; set; }

        public static AlertContentEntity Parse(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return null;
            }

            string[] strs = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs == null || strs.Length != 2)
            {
                return null;
            }

            int aimHour;
            if (!int.TryParse(strs[0], out aimHour))
            {
                return null;
            }

            AlertContentEntity info = new AlertContentEntity()
            {
                Content = strs[1],
                AimHour = aimHour
            };

            if (!info.IsValid)
            {
                return null;
            }

            return info;
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Content) && AimHour >= 0 && AimHour <= 23;
            }
        }
    }
}
