using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;
using System.IO;
using AILib;
using AILib.Entities;

namespace AutoRestart
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer timer = new System.Timers.Timer();
        private string CQPRootPath = @".\";

        private string ProcessName = "CQP";

        private bool IsRunning = true;

        private int MissHeartCount = 0;
        private int MaxMissLimit = 4;
        private int CheckFrequency = 30;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Enabled = true;
            timer.Interval = CheckFrequency * 1000;
            timer.AutoReset = true;
            timer.Elapsed += TimeUp;

            timer.Start();
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            Restart();

            CheckHeartBeat();

            ClearOldPicCache();
        }

        private void CheckHeartBeat()
        {
            var query = DbMgr.Query<HeartBeatEntity>();
            if (query == null || query.Count() == 0 || query.FirstOrDefault().LastBeatTime < DateTime.Now.AddSeconds(-CheckFrequency))
            {
                MissHeartCount++;
            }

            if (MissHeartCount > MaxMissLimit)
            {
                KillCQ();
                MissHeartCount = 0;
            }
        }

        private void KillCQ()
        {
            Process[] processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName == ProcessName)
                {
                    p.Kill();
                    LogEntity.Log($"[Kill] {DateTime.Now}");

                    Restart();
                    return;
                }
            }
        }

        private bool IsCQRunning()
        {
            Process[] processes = Process.GetProcesses();
            if (processes == null || processes.Length == 0)
            {
                return false;
            }

            foreach (var p in processes)
            {
                if (p.ProcessName == ProcessName)
                {
                    return true;
                }
            }

            return false;
        }

        private void Restart()
        {
            if (IsCQRunning())
            {
                return;
            }

            ProcessStartInfo psInfo = new ProcessStartInfo(CQPRootPath + "QuickStart.lnk");
            Process.Start(psInfo);

            LogEntity.Log($"[Restart] {DateTime.Now}");

            this.Invoke(new Action(() =>
            {
                dataGridView1.DataSource = DbMgr.Query<LogEntity>();
                dataGridView1.Refresh();
            }));
        }

        private void ClearOldPicCache()
        {
            string picCachePath = CQPRootPath + @"\data\image\";

            DirectoryInfo dir = new DirectoryInfo(picCachePath);
            foreach (var f in dir.GetFiles())
            {
                if (f.Extension == ".cqimg" && f.CreationTime <= DateTime.Now.AddHours(-1))
                {
                    f.Delete();
                }
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.QPan_OpenFromTuoPan();
        }

        private void QPan_OpenFromTuoPan()
        {
            this.Visible = true;
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.notifyIcon1.Visible = true;
        }

        private void MenuItemOpen_Click(object sender, EventArgs e)
        {
            this.QPan_OpenFromTuoPan();
        }

        private void MenuItemClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
            {
                this.QPan_MiniMizedToTuoPan();
            }
        }

        private void QPan_MiniMizedToTuoPan()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }

        private void ppMenuItem_Click(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                ppMenuItem.Text = "继续";
                timer.Stop();
                IsRunning = false;
            }
            else
            {
                ppMenuItem.Text = "暂停";
                timer.Start();
                IsRunning = true;
            }
        }
    }
}