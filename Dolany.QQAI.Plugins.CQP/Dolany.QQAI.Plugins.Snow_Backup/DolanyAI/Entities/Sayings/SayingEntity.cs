using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.Snow.DolanyAI
{
    public class SayingEntity : EntityBase
    {
        [DataColumn]
        public string Cartoon { get; set; }

        [DataColumn]
        public string Charactor { get; set; }

        [DataColumn]
        public long FromGroup { get; set; }

        public static SayingEntity Parse(string Msg)
        {
            if (string.IsNullOrEmpty(Msg))
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

                SayingEntity si = new SayingEntity()
                {
                    Cartoon = parts[0],
                    Charactor = parts[1],
                    Content = parts[2]
                };
                if (si.IsValid)
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
                return !string.IsNullOrEmpty(Cartoon) && !string.IsNullOrEmpty(Charactor) && !string.IsNullOrEmpty(Content);
            }
        }

        public bool Contains(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return true;
            }

            return Cartoon.Contains(keyword) || Charactor.Contains(keyword) || Content.Contains(keyword);
        }
    }
}