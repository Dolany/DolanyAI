namespace Dolany.Ice.Ai.DolanyAI
{
    public class HourAlertChecker : ISyntaxChecker
    {
        public bool Check(string msg, out object[] param)
        {
            param = null;
            var info = AlertContentExtension.Parse(msg);
            if (info == null)
            {
                return false;
            }

            param = new object[] { info };
            return true;
        }
    }
}