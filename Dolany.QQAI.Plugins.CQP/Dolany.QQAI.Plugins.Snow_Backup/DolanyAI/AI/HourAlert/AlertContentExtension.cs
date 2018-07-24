using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.QQAI.Plugins.Snow.DolanyAI.Db;

namespace Dolany.QQAI.Plugins.Snow.DolanyAI
{
    public static class AlertContentExtension
    {
        public static AlertContent Parse(string msg)
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

            AlertContent info = new AlertContent()
            {
                Content = strs[1],
                AimHour = aimHour
            };

            if (!info.IsValid())
            {
                return null;
            }

            return info;
        }

        public static bool IsValid(this AlertContent ac)
        {
            return !string.IsNullOrEmpty(ac.Content) && ac.AimHour >= 0 && ac.AimHour <= 23;
        }
    }
}