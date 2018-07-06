using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DolanyToolControl;

namespace CQPMonitor.Tools
{
    public class ToolBase
    {
        public string Name { get; set; }
        public string Decription { get; set; }
        public string Icon { get; set; }
        public bool IsAutoStart { get; set; }
        public Form RelatedForm { get; set; }
        public dolanyToolCon RelatedControl { get; set; }

        public virtual void Work()
        {
            
        }

        public virtual void Show()
        {
            if (RelatedForm != null)
            {
                RelatedForm.StartPosition = FormStartPosition.CenterParent;
                RelatedForm.Show();
            }
        }
    }
}