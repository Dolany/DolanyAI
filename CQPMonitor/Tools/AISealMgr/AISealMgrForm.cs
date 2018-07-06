using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace CQPMonitor.Tools.AISealMgr
{
    public partial class AISealMgrForm : Form
    {
        private int ImageMaxCache = 200;
        private string CQPRootPath = @".\";

        public AISealMgrForm()
        {
            InitializeComponent();
        }

        private void ClearOldPicCache()
        {
            string picCachePath = CQPRootPath + @"\data\image\";

            DirectoryInfo dir = new DirectoryInfo(picCachePath);
            var query = dir.GetFiles().OrderByDescending(p => p.CreationTime);
            var imageCacheList = query.ToList();
            for (int i = ImageMaxCache; i < imageCacheList.Count; i++)
            {
                imageCacheList[i].Delete();
            }
        }
    }
}