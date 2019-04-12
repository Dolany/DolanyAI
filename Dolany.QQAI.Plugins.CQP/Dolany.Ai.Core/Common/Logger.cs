using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Common
{
    using System;

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

            MsgSender.PushMsg(0, Utility.DeveloperNumber, error);
        }
    }
}
