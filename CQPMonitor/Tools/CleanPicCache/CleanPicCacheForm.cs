using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;

namespace CQPMonitor.Tools.CleanPicCache
{
    [Tool(
        ToolName = "清理图片缓存",
        Decription = "清理图片缓存",
        ToolIcon = "cleancache.ico",
        IsAutoStart = false,
        Order = 2
        )]
    public partial class CleanPicCacheForm : ToolBaseForm
    {
        private string CachePath = "./data/image/";

        private int MaxCache = 100;
        private int CleanFreq = 5;

        private readonly System.Timers.Timer timer = new System.Timers.Timer();

        public CleanPicCacheForm()
            : base()
        {
            InitializeComponent();

            //Init();
        }

        public void Init()
        {
            var MaxCache_Config = Utility.GetConfig("MaxPicCacheCount");
            if (!string.IsNullOrEmpty(MaxCache_Config))
            {
                MaxCache = int.Parse(MaxCache_Config);
            }

            var CleanFreq_Config = Utility.GetConfig("CleanFreq");
            if (!string.IsNullOrEmpty(CleanFreq_Config))
            {
                CleanFreq = int.Parse(CleanFreq_Config);
            }

            timer.Interval = CleanFreq * 1000;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += TimeUp;

            if (ToolAttr.IsAutoStart)
            {
                timer.Start();
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
            else
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
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
                Utility.SendMsgToDeveloper(ex);
            }
        }

        private void CleanCache()
        {
            var dir = new DirectoryInfo(CachePath);
            var cleanCount = dir.GetFiles().Count() - MaxCache;
            var cleanFiles = dir.GetFiles().OrderBy(f => f.CreationTime).Take(cleanCount);
            foreach (var f in cleanFiles)
            {
                f.Delete();
            }

            RefreshCacheCount();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var MaxCacheCountStr = MaxCacheCountTxt.Text;
            var CleanFreqStr = CleanFreqTxt.Text;

            if (!int.TryParse(MaxCacheCountStr, out int m) || !int.TryParse(CleanFreqStr, out int c))
            {
                MessageBox.Show("输入不合法！");
                return;
            }

            MaxCache = m;
            CleanFreq = c;

            Utility.SetConfig("MaxPicCacheCount", MaxCache.ToString());
            Utility.SetConfig(nameof(CleanFreq), CleanFreq.ToString());

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

            RefreshCacheCount();
        }

        public override void Shutdown()
        {
            base.Shutdown();

            timer.Stop();
            timer.Enabled = false;
        }

        private void RefreshCacheCount()
        {
            var dir = new DirectoryInfo(CachePath);
            CurCacheCountLbl.Text = dir.GetFiles().Count().ToString();
        }
    }
}