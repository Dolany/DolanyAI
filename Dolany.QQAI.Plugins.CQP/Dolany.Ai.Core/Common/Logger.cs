namespace Dolany.Ai.Core.Common
{
    using System;

    public static class Logger
    {
        public static void Log(string log)
        {
            AIMgr.Instance.MessagePublish(log);
            Dolany.Ai.Common.RuntimeLogger.Log(log);
        }

        public static void Log(Exception ex)
        {
            Sys_ErrorCount.Plus(ex.Message + "\r\n" + ex.StackTrace);
            Dolany.Ai.Common.RuntimeLogger.Log(ex);
        }
    }
}
