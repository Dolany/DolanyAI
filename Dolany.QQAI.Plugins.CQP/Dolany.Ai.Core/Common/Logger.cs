using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using System;

namespace Dolany.Ai.Core.Common
{
    public static class Logger
    {
        public static void Log(string log)
        {
            AIMgr.Instance.MessagePublish(log);
            RuntimeLogger.Log(log);
        }

        public static void Log(Exception ex)
        {
            var error = ex.Message + "\r\n" + ex.StackTrace;
            AIAnalyzer.AddError(error);
            AIMgr.Instance.MessagePublish(error);
            RuntimeLogger.Log(ex);

            MsgSender.PushMsg(0, Global.DeveloperNumber, error, Configger.Instance.AIConfig.MainAi);
        }
    }
}
