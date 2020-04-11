using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using System;

namespace Dolany.Ai.Core.Common
{
    public static class Logger
    {
        public static void Log(string log)
        {
            Global.MsgPublish(log);
            RuntimeLogger.Log(log);
        }

        public static void Log(Exception ex)
        {
            var error = ex.Message + "\r\n" + ex.StackTrace;
            AIAnalyzer.AddError(error);
            Global.MsgPublish(error);
            RuntimeLogger.Log(ex);

            MsgSender.PushMsg(0, Global.DeveloperNumber, error, Global.DefaultConfig.MainAi);
        }
    }
}
