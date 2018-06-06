using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public class MemoInfo
    {
        public string Memo { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? MemoTime { get; set; }
        public long Creator { get; set; }
        public long FromGroup { get; set; }

        public static MemoInfo Parse(string msg)
        {
            if(!msg.StartsWith("备忘 "))
            {
                return null;
            }

            msg = msg.Replace("备忘 ", "");
            string[] strs = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if(strs == null || strs.Length != 2)
            {
                return null;
            }

            if(strs[0].EndsWith("后"))
            {
                return null;
            }

            string MemoTime_str = strs[0].Replace("后", "");
            return new MemoInfo()
            {
                Memo = strs[1],
                MemoTime = ParseAfterTime(MemoTime_str),
                CreateTime = DateTime.Now
            };
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Memo) && MemoTime != null;
            }
        }

        private static DateTime ParseAfterTime(string timeStr)
        {
            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;

            year = ParseTimeSpan(ref timeStr, "年");

            if((month = ParseTimeSpan(ref timeStr, "个月")) == 0)
            {
                month = ParseTimeSpan(ref timeStr, "月");
            }

            if((day = ParseTimeSpan(ref timeStr, "天")) == 0)
            {
                day = ParseTimeSpan(ref timeStr, "日");
            }

            if((hour = ParseTimeSpan(ref timeStr, "个小时")) == 0)
            {
                if((hour = ParseTimeSpan(ref timeStr, "小时")) == 0)
                {
                    hour = ParseTimeSpan(ref timeStr, "时");
                }
            }

            if ((minute = ParseTimeSpan(ref timeStr, "分钟")) == 0)
            {
                minute = ParseTimeSpan(ref timeStr, "分");
            }

            if ((second = ParseTimeSpan(ref timeStr, "秒钟")) == 0)
            {
                second = ParseTimeSpan(ref timeStr, "秒");
            }

            return DateTime.Now.AddYears(year)
                .AddMonths(month)
                .AddDays(day)
                .AddHours(hour)
                .AddMinutes(minute)
                .AddSeconds(second);
        }

        private static int ParseTimeSpan(ref string timeStr, string spanName)
        {
            if(!timeStr.Contains(spanName))
            {
                return 0;
            }

            string[] strs = timeStr.Split(new string[] { spanName }, StringSplitOptions.RemoveEmptyEntries);
            if(strs == null || strs.Length != 2)
            {
                return 0;
            }

            int timespan;
            if(!int.TryParse(strs[0], out timespan))
            {
                return 0;
            }

            timeStr = strs[1];
            return timespan;
        }
    }
}
