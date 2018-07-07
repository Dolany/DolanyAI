using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AILib;
using System.Timers;
using System.IO;

namespace CQPMonitor.Tools.CleanPicCache
{
    public partial class CleanPicCacheForm : ToolBaseForm
    {
        private string CachePath = "./data/image/";

        private int MaxCache = 100;
        private int CleanFreq = 5;

        private System.Timers.Timer timer = new System.Timers.Timer();

        public CleanPicCacheForm()
            : base()
        {
            ToolName = "清理图片缓存";
            Decription = "清理图片缓存";
            ToolIcon = "cleancache.ico";
            IsAutoStart = true;
            Order = 2;

            InitializeComponent();

            Init();
        }

        public void Init()
        {
            string MaxCache_Config = Common.GetConfig("MaxPicCacheCount");
            if (!string.IsNullOrEmpty(MaxCache_Config))
            {
                MaxCache = int.Parse(MaxCache_Config);
            }

            string CleanFreq_Config = Common.GetConfig("CleanFreq");
            if (!string.IsNullOrEmpty(CleanFreq_Config))
            {
                CleanFreq = int.Parse(CleanFreq_Config);
            }
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            try
            {
                CleanCache();
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        private void CleanCache()
        {
            DirectoryInfo dir = new DirectoryInfo(CachePath);
            int cleanCount = dir.GetFiles().Count() - MaxCache;
            var cleanFiles = dir.GetFiles().OrderBy(f => f.CreationTime).Take(cleanCount);
            foreach (var f in cleanFiles)
            {
                f.Delete();
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            string MaxCacheCountStr = MaxCacheCountTxt.Text;
            string CleanFreqStr = CleanFreqTxt.Text;

            int m, c;
            if (!int.TryParse(MaxCacheCountStr, out m) || !int.TryParse(CleanFreqStr, out c))
            {
                MessageBox.Show("输入不合法！");
                return;
            }

            MaxCache = m;
            CleanFreq = c;

            Common.SetConfig("MaxPicCacheCount", MaxCache.ToString());
            Common.SetConfig("CleanFreq", CleanFreq.ToString());

            MessageBox.Show("保存成功！");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CleanCache();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }

        private void CleanPicCacheForm_Load(object sender, EventArgs e)
        {
            MaxCacheCountTxt.Text = MaxCache.ToString();
            CleanFreqTxt.Text = CleanFreq.ToString();
        }
    }
}