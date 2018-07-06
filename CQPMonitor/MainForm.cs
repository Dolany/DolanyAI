﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;





namespace CQPMonitor
{
    public partial class MainForm : Form
    {
        

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        

        private void ClearOldPicCache()
        {
            //string picCachePath = CQPRootPath + @"\data\image\";

            //DirectoryInfo dir = new DirectoryInfo(picCachePath);
            //var query = dir.GetFiles().OrderByDescending(p => p.CreationTime);
            //var imageCacheList = query.ToList();
            //for (int i = ImageMaxCache; i < imageCacheList.Count; i++)
            //{
            //    imageCacheList[i].Delete();
            //}
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
            //if (IsRunning)
            //{
            //    ppMenuItem.Text = "继续";
            //    timer.Stop();
            //    IsRunning = false;
            //}
            //else
            //{
            //    ppMenuItem.Text = "暂停";
            //    timer.Start();
            //    IsRunning = true;
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定要关闭程序吗？", "注意", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void restartBtn_Click(object sender, EventArgs e)
        {
            
        }
    }
}