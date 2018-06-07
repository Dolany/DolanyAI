using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.AI.Interaction.Publisher
{
    public class PublishRecordInfo
    {
        public string PublishContent { get; set; }
        public string Index { get; set; }
        public DateTime? CreateTime { get; set; }

        public static PublishRecordInfo Parse(string msg)
        {
            if(string.IsNullOrEmpty(msg) || !msg.StartsWith("Publish "))
            {
                return null;
            }

            msg = msg.Replace("Publish ", "");
            if(string.IsNullOrEmpty(msg))
            {
                return null;
            }

            return new PublishRecordInfo()
            {
                PublishContent = msg,
                Index = DateTime.Now.ToString("yyyyMMddHHmmss"),
                CreateTime = DateTime.Now
            };
        }
    }
}
