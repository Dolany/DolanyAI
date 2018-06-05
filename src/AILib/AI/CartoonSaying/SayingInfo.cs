using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public class SayingInfo
    {
        public string Cartoon { get; set; }

        public string Charactor { get; set; }

        public string Sayings { get; set; }

        public static SayingInfo Parse(string Msg)
        {
            if(string.IsNullOrEmpty(Msg))
            {
                return null;
            }

            try
            {
                string[] parts = Msg.Split(new char[] { ':', '：', ' ' });
                if (parts.Length < 4 || !parts[0].Contains("语录"))
                {
                    return null;
                }

                return new SayingInfo()
                {
                    Cartoon = parts[1],
                    Charactor = parts[2],
                    Sayings = parts[3]
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
