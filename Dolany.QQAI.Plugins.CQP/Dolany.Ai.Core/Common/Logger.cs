using Dolany.Ai.Common;

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
            AIAnalyzer.AddError(ex.Message + "\r\n" + ex.StackTrace);
            AIMgr.Instance.MessagePublish(ex.Message + "\r\n" + ex.StackTrace);
            RuntimeLogger.Log(ex);
        }
    }
}
