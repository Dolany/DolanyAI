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
using AILib.Entities;
using System.Diagnostics;
using System.Timers;

namespace CQPMonitor.Tools.AutoRestart
{
    public partial class AutoRestartForm : Form
    {
        private System.Timers.Timer timer = new System.Timers.Timer();
        private string CQPRootPath = @".\";

        private string ProcessName = "CQP";

        private bool IsRunning = true;

        private int MissHeartCount = 0;
        private int MaxMissLimit = 4;
        private int CheckFrequency = 30;

        private List<LogEntity> Logs;

        public AutoRestartForm()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            timer.Enabled = true;
            timer.Interval = CheckFrequency * 1000;
            timer.AutoReset = true;
            timer.Elapsed += TimeUp;

            timer.Start();

            RefreshTable();
        }

        private void TimeUp(object sender, ElapsedEventArgs e)
        {
            try
            {
                Restart();

                CheckHeartBeat();
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        private void CheckHeartBeat()
        {
            var query = DbMgr.Query<HeartBeatEntity>();
            if (query.IsNullOrEmpty() || query.FirstOrDefault().LastBeatTime < DateTime.Now.AddSeconds(-CheckFrequency))
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
                    KeyLogger.Log($"[Kill] {DateTime.Now}", "Restart");

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

            KeyLogger.Log($"[Restart] {DateTime.Now}", "Restart");

            RefreshTable();
        }

        private void RefreshTable()
        {
            this.Invoke(new Action(() =>
            {
                var query = DbMgr.Query<LogEntity>(l => l.LogType == "Restart");
                Logs = query.OrderByDescending(l => l.CreateTime).ToList();
                ShowTable.DataSource = Logs;
                ShowTable.Refresh();
            }));
        }

        private void 删除选中行的数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowTable.CurrentRow == null || ShowTable.CurrentRow.Index < 0)
            {
                return;
            }

            DbMgr.Delete<LogEntity>(Logs[ShowTable.CurrentRow.Index].Id);
        }

        private void RestartBtn_Click(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void RefreshTableBtn_Click(object sender, EventArgs e)
        {
            KillCQ();
        }
    }
}