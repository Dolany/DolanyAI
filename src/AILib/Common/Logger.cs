using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.Entities;

namespace AILib
{
    public static class Logger
    {
        public static void Log(string msg)
        {
            LogEntity log = new LogEntity()
            {
                Id = Guid.NewGuid().ToString(),
                Content = msg,
                LogType = "默认",
                CreateTime = DateTime.Now
            };

            DbMgr.Insert(log);
        }

        public static void Log(string msg, string logType)
        {
            LogEntity log = new LogEntity()
            {
                Id = Guid.NewGuid().ToString(),
                Content = msg,
                LogType = logType,
                CreateTime = DateTime.Now
            };

            DbMgr.Insert(log);
        }
    }
}