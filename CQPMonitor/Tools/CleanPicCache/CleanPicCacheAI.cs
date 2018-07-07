using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQPMonitor.Tools.CleanPicCache;

namespace CQPMonitor.Tools
{
    public class CleanPicCacheAI : ToolBase
    {
        public CleanPicCacheAI()
        {
            Name = "清理图片缓存";
            Decription = "清理图片缓存";
            Icon = "cleancache.ico";
            IsAutoStart = true;
            RelatedForm = new CleanPicCacheForm(IsAutoStart);
        }

        public override void Work()
        {
            base.Work();
        }
    }
}