﻿using System;
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
    public partial class AutoRestartForm : ToolBaseForm
    {
        private System.Timers.Timer timer = new System.Timers.Timer();
        private string CQPRootPath = @".\";

        private string ProcessName = "CQP";

        private int MissHeartCount = 0;
        private int MaxMissLimit = 4;
        private int CheckFrequency = 30;
        private int LogShowCount = 30;

        private List<LogEntity> Logs;

        public AutoRestartForm()
        {
            ToolName = "自动重启";
            Decription = "自动重启CQP，检测历史重启情况";
            ToolIcon = "autorestart.ico";
            IsAutoStart = true;
            Order = 1;

            InitializeComponent();

            InitUI();
            InitTimer();
        }

        private void InitTimer()
        {
            timer.Enabled = true;
            timer.Interval = CheckFrequency * 1000;
            timer.AutoReset = true;
            timer.Elapsed += TimeUp;

            if (IsAutoStart)
            {
                timer.Start();
            }
        }

        private void InitUI()
        {
            radioButton1.Checked = IsAutoStart;
            radioButton2.Checked = !IsAutoStart;

            string MaxMissLimit_Config = Common.GetConfig("MaxMissLimit");
            if (!string.IsNullOrEmpty(MaxMissLimit_Config))
            {
                MaxMissLimit = int.Parse(MaxMissLimit_Config);
            }

            string CheckFrequency_Config = Common.GetConfig("CheckFrequency");
            if (!string.IsNullOrEmpty(CheckFrequency_Config))
            {
                CheckFrequency = int.Parse(CheckFrequency_Config);
            }

            string LogShowCount_Config = Common.GetConfig("LogShowCount");
            if (!string.IsNullOrEmpty(LogShowCount_Config))
            {
                LogShowCount = int.Parse(LogShowCount_Config);
            }
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
                Logs = query.OrderByDescending(l => l.CreateTime).Take(LogShowCount).ToList();
                //ShowTable.DataSource = Logs.Select(l => (l.CreateTime, l.LogType, l.Content)).ToList();
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
            RefreshTable();
        }

        private void RestartBtn_Click(object sender, EventArgs e)
        {
            KillCQ();
            Restart();
        }

        private void RefreshTableBtn_Click(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void AutoRestartForm_Load(object sender, EventArgs e)
        {
            RefreshTable();
            RefreshTxt();
        }

        private void RefreshTxt()
        {
            RefreshFreqTxt.Text = CheckFrequency.ToString();
            MaxMissCountTxt.Text = MaxMissLimit.ToString();
            LogShowCountTxt.Text = LogShowCount.ToString();

            timer.Interval = CheckFrequency * 1000;
        }

        public override void Shutdown()
        {
            base.Shutdown();

            timer.Stop();
            timer.Enabled = false;
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

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            int m, c, l;
            if (!int.TryParse(RefreshFreqTxt.Text, out c)
                || !int.TryParse(MaxMissCountTxt.Text, out m)
                || !int.TryParse(LogShowCountTxt.Text, out l))
            {
                MessageBox.Show("输入不合法！");
                return;
            }

            CheckFrequency = c;
            MaxMissLimit = m;
            LogShowCount = l;
            RefreshTxt();

            Common.SetConfig("MaxMissLimit", MaxMissLimit.ToString());
            Common.SetConfig("CheckFrequency", CheckFrequency.ToString());
            Common.SetConfig("LogShowCount", LogShowCount.ToString());

            MessageBox.Show("保存成功！");
        }
    }
}