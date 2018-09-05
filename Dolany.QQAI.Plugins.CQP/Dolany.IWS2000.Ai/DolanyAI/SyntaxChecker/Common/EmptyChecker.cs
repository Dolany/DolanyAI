﻿namespace Dolany.IWS2000.Ai.DolanyAI
{
    public class EmptyChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            return string.IsNullOrEmpty(msg);
        }
    }
}