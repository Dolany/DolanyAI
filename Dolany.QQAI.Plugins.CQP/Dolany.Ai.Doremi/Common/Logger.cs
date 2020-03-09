﻿using System;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi.Cache;

namespace Dolany.Ai.Doremi.Common
{
    public static class Logger
    {
        private static AISvc AiSvc => AutofacSvc.Resolve<AISvc>();

        public static void Log(string log)
        {
            AiSvc.MessagePublish(log);
            RuntimeLogger.Log(log);
        }

        public static void Log(Exception ex)
        {
            var error = ex.Message + "\r\n" + ex.StackTrace;
            AIAnalyzer.AddError(error);
            AiSvc.MessagePublish(error);
            RuntimeLogger.Log(ex);

            MsgSender.PushMsg(0, Global.DeveloperNumber, error, Configger<AIConfigBase>.Instance.AIConfig.MainAi);
        }
    }
}
