using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    public class SayingInfo
    {
        /// <summary>
        /// 出处
        /// </summary>
        public string Cartoon { get; set; }

        /// <summary>
        /// 人物
        /// </summary>
        public string Charactor { get; set; }

        /// <summary>
        /// 语录
        /// </summary>
        public string Sayings { get; set; }

        public long FromGroup { get; set; }

        public static SayingInfo Parse(string Msg)
        {
            if(string.IsNullOrEmpty(Msg))
            {
                return null;
            }

            try
            {
                string[] parts = Msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                {
                    return null;
                }

                SayingInfo si = new SayingInfo()
                {
                    Cartoon = parts[0],
                    Charactor = parts[1],
                    Sayings = parts[2]
                };
                if(si.IsValid)
                {
                    return si;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Cartoon) && !string.IsNullOrEmpty(Charactor) && !string.IsNullOrEmpty(Sayings);
            }
        }

        public bool Contains(string keyword)
        {
            if(string.IsNullOrEmpty(keyword))
            {
                return true;
            }

            return Cartoon.Contains(keyword) || Charactor.Contains(keyword) || Sayings.Contains(keyword);
        }
    }
}
