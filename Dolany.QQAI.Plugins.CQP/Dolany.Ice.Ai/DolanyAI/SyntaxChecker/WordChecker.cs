﻿namespace Dolany.Ice.Ai.DolanyAI
{
    public class WordChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            param = new object[] { msg };
            return true;
        }
    }
}