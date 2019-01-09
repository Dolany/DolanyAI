using System;

namespace Dolany.Ai.Core.Ai.Record
{
    using Dolany.Database.Ai;

    public static class AlertContentExtension
    {
        public static AlertContent Parse(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return null;
            }

            var strs = msg.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length != 2)
            {
                return null;
            }

            if (!int.TryParse(strs[0], out var aimHour))
            {
                return null;
            }

            var info = new AlertContent
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
