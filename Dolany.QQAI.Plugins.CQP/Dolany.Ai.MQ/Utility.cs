using System.Linq;

using Dolany.Ai.Util;

namespace Dolany.Ai.MQ
{
    using System.IO;

    public static class Utility
    {
        private static string AuthCode;

        public static string GetAuthCode()
        {
            if (!AuthCode.IsNullOrEmpty())
            {
                return AuthCode;
            }

            const string logPath = "./logs/";
            var dir = new DirectoryInfo(logPath);

            foreach (var file in dir.GetFiles().OrderByDescending(p => p.CreationTime))
            {
                var authLine = "";
                using (var reader = new StreamReader(file.FullName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("\"AuthCode\":"))
                        {
                            authLine = line;
                        }
                    }
                }

                if (authLine.IsNullOrEmpty())
                {
                    continue;
                }

                var strs = authLine.Split('\"');
                AuthCode = strs[3];
                break;
            }

            return AuthCode;
        }
    }
}
