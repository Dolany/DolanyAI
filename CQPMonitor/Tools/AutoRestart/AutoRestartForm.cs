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
using Dolany.Ice.Ai.DolanyAI;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace CQPMonitor.Tools.AutoRestart
{
    [Tool(
        ToolName = "自动重启",
        Decription = "自动重启机器人，检测历史重启情况",
        ToolIcon = "autorestart.ico",
        IsAutoStart = true,
        Order = 1
        )]
    public partial class AutoRestartForm : ToolBaseForm
    {
        private System.Timers.Timer timer = new System.Timers.Timer();
        private string AIRootPath = @"C:\AmandaQQ\";

        private string ProcessName = "Amanda";

        private int MissHeartCount = 0;

        private int MaxMissLimit
        {
            get
            {
                var config = Utility.GetConfig("MaxMissLimit");
                if (string.IsNullOrEmpty(config))
                {
                    Utility.SetConfig("MaxMissLimit", "5");
                    return 5;
                }

                return int.Parse(config);
            }
        }

        public int CheckFrequency
        {
            get
            {
                var c = Utility.GetConfig("CheckFrequency");
                if (string.IsNullOrEmpty(c))
                {
                    Utility.SetConfig("CheckFrequency", "10");
                    return 10;
                }

                return int.Parse(c);
            }
        }

        public DateTime? HeartBeat
        {
            get
            {
                var c = Utility.GetConfig("HeartBeat");
                if (string.IsNullOrEmpty(c))
                {
                    Utility.SetConfig("HeartBeat", "");
                    return null;
                }

                return DateTime.Parse(c);
            }
        }

        private int LogShowCount = 30;

        private bool IsLoaded = false;

        public AutoRestartForm()
        {
            InitializeComponent();

            //InitUI();
            InitTimer();
        }

        private void InitTimer()
        {
            timer.Enabled = true;
            timer.Interval = CheckFrequency * 1000;
            timer.AutoReset = true;
            timer.Elapsed += TimeUp;

            if (ToolAttr.IsAutoStart)
            {
                timer.Start();
            }
        }

        private void InitUI()
        {
            radioButton1.Checked = ToolAttr.IsAutoStart;
            radioButton2.Checked = !ToolAttr.IsAutoStart;

            string LogShowCount_Config = Utility.GetConfig("LogShowCount");
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
                //ProcessMonitor();
                //SetRestartCount();
            }
            catch (Exception ex)
            {
                Utility.SendMsgToDeveloper(ex);
            }
        }

        private void SetRestartCount()
        {
            //var query = DbMgr.Query<LogEntity>(l =>
            //    l.LogType == "Restart"
            //    && l.Content.Contains("Restart")
            //    && l.CreateTime >= DateTime.Now.AddDays(-1)
            //    );

            //int count = query.IsNullOrEmpty() ? 0 : query.Count();
            //RestartCountLbl.Text = count.ToString();
        }

        private void SetState(string state)
        {
            if (!IsLoaded)
            {
                return;
            }

            Invoke(new Action(() =>
            {
                CurStateLbl.Text = state;
            }));
        }

        private void KillCQ()
        {
            SetState("Kill");
            var processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName == ProcessName)
                {
                    p.Kill();

                    Restart();
                    return;
                }
            }
        }

        private bool IsAIRunning()
        {
            if (HeartBeat == null || HeartBeat.Value.AddSeconds(CheckFrequency) < DateTime.Now)
            {
                MissHeartCount++;
            }

            if (MissHeartCount > MaxMissLimit)
            {
                MissHeartCount = 0;
                return false;
            }

            return true;
        }

        private void Restart()
        {
            if (IsAIRunning())
            {
                return;
            }

            KillCQ();
            SetState(nameof(Restart));
            var psInfo = new ProcessStartInfo(AIRootPath + "Amanda.exe");
            Process.Start(psInfo);

            RefreshTable();
            SetState("正常");
        }

        private static void RefreshTable()
        {
        }

        private static void 删除选中行的数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void RestartBtn_Click(object sender, EventArgs e)
        {
            KillCQ();
            Restart();
        }

        private static void RefreshTableBtn_Click(object sender, EventArgs e)
        {
            //RefreshTable();
        }

        private void AutoRestartForm_Load(object sender, EventArgs e)
        {
            IsLoaded = true;
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

        private static void SaveBtn_Click(object sender, EventArgs e)
        {
        }
    }
}