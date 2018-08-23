namespace Dolany.Ice.Ai.DolanyAI
{
    public class ProcceedMsgChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            var info = SayingsExtention.Parse(msg);
            if (info != null)
            {
                param = new object[] { 1, info };
                return true;
            }

            var keyword = string.IsNullOrEmpty(msg.Trim()) ? null : msg;
            if (string.IsNullOrEmpty(keyword))
            {
                param = new object[] { 2 };
                return true;
            }

            param = new object[] { 3, keyword };
            return true;
        }
    }
}