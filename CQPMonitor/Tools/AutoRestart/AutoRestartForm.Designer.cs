namespace CQPMonitor.Tools.AutoRestart
{
    partial class AutoRestartForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除选中行的数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowTable = new System.Windows.Forms.DataGridView();
            this.CreateTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RestartBtn = new System.Windows.Forms.Button();
            this.RefreshTableBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.RestartCountLbl = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CurStateLbl = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.LogShowCountTxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.MaxMissCountTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.RefreshFreqTxt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ShowTable)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除选中行的数据ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 26);
            // 
            // 删除选中行的数据ToolStripMenuItem
            // 
            this.删除选中行的数据ToolStripMenuItem.Name = "删除选中行的数据ToolStripMenuItem";
            this.删除选中行的数据ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.删除选中行的数据ToolStripMenuItem.Text = "删除选中行的数据";
            this.删除选中行的数据ToolStripMenuItem.Click += new System.EventHandler(this.删除选中行的数据ToolStripMenuItem_Click);
            // 
            // ShowTable
            // 
            this.ShowTable.AllowUserToAddRows = false;
            this.ShowTable.AllowUserToDeleteRows = false;
            this.ShowTable.AllowUserToResizeRows = false;
            this.ShowTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ShowTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CreateTimeColumn,
            this.TypeColumn,
            this.ContentColumn});
            this.ShowTable.ContextMenuStrip = this.contextMenuStrip1;
            this.ShowTable.Location = new System.Drawing.Point(186, 2);
            this.ShowTable.MultiSelect = false;
            this.ShowTable.Name = "ShowTable";
            this.ShowTable.ReadOnly = true;
            this.ShowTable.RowHeadersVisible = false;
            this.ShowTable.RowTemplate.Height = 23;
            this.ShowTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ShowTable.Size = new System.Drawing.Size(403, 447);
            this.ShowTable.TabIndex = 10;
            // 
            // CreateTimeColumn
            // 
            this.CreateTimeColumn.DataPropertyName = "CreateTime";
            this.CreateTimeColumn.Frozen = true;
            this.CreateTimeColumn.HeaderText = "时间";
            this.CreateTimeColumn.Name = "CreateTimeColumn";
            this.CreateTimeColumn.ReadOnly = true;
            // 
            // TypeColumn
            // 
            this.TypeColumn.DataPropertyName = "LogType";
            this.TypeColumn.Frozen = true;
            this.TypeColumn.HeaderText = "类型";
            this.TypeColumn.Name = "TypeColumn";
            this.TypeColumn.ReadOnly = true;
            // 
            // ContentColumn
            // 
            this.ContentColumn.DataPropertyName = "Content";
            this.ContentColumn.HeaderText = "内容";
            this.ContentColumn.Name = "ContentColumn";
            this.ContentColumn.ReadOnly = true;
            this.ContentColumn.Width = 200;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RestartBtn);
            this.groupBox1.Controls.Add(this.RefreshTableBtn);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 100);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作";
            // 
            // RestartBtn
            // 
            this.RestartBtn.Location = new System.Drawing.Point(0, 20);
            this.RestartBtn.Name = "RestartBtn";
            this.RestartBtn.Size = new System.Drawing.Size(75, 21);
            this.RestartBtn.TabIndex = 10;
            this.RestartBtn.Text = "重启";
            this.RestartBtn.UseVisualStyleBackColor = true;
            this.RestartBtn.Click += new System.EventHandler(this.RestartBtn_Click);
            // 
            // RefreshTableBtn
            // 
            this.RefreshTableBtn.Location = new System.Drawing.Point(93, 20);
            this.RefreshTableBtn.Name = "RefreshTableBtn";
            this.RefreshTableBtn.Size = new System.Drawing.Size(75, 21);
            this.RefreshTableBtn.TabIndex = 11;
            this.RefreshTableBtn.Text = "刷新";
            this.RefreshTableBtn.UseVisualStyleBackColor = true;
            this.RefreshTableBtn.Click += new System.EventHandler(this.RefreshTableBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(0, 50);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(168, 36);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "自动重启";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(67, 14);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(35, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "否";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 14);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(35, 16);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "是";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.RestartCountLbl);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.CurStateLbl);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(168, 72);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "监视";
            // 
            // RestartCountLbl
            // 
            this.RestartCountLbl.AutoSize = true;
            this.RestartCountLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RestartCountLbl.Location = new System.Drawing.Point(101, 45);
            this.RestartCountLbl.Name = "RestartCountLbl";
            this.RestartCountLbl.Size = new System.Drawing.Size(2, 14);
            this.RestartCountLbl.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "24小时重启计数";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "当前状态：";
            // 
            // CurStateLbl
            // 
            this.CurStateLbl.AutoSize = true;
            this.CurStateLbl.Location = new System.Drawing.Point(100, 17);
            this.CurStateLbl.Name = "CurStateLbl";
            this.CurStateLbl.Size = new System.Drawing.Size(29, 12);
            this.CurStateLbl.TabIndex = 8;
            this.CurStateLbl.Text = "正常";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.SaveBtn);
            this.groupBox4.Controls.Add(this.LogShowCountTxt);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.MaxMissCountTxt);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.RefreshFreqTxt);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(12, 191);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(168, 258);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "设置";
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(87, 229);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveBtn.TabIndex = 6;
            this.SaveBtn.Text = "保存";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // LogShowCountTxt
            // 
            this.LogShowCountTxt.Location = new System.Drawing.Point(83, 88);
            this.LogShowCountTxt.Name = "LogShowCountTxt";
            this.LogShowCountTxt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.LogShowCountTxt.Size = new System.Drawing.Size(79, 21);
            this.LogShowCountTxt.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "日志显示行数";
            // 
            // MaxMissCountTxt
            // 
            this.MaxMissCountTxt.Location = new System.Drawing.Point(83, 58);
            this.MaxMissCountTxt.Name = "MaxMissCountTxt";
            this.MaxMissCountTxt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.MaxMissCountTxt.Size = new System.Drawing.Size(79, 21);
            this.MaxMissCountTxt.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "最大失联计数";
            // 
            // RefreshFreqTxt
            // 
            this.RefreshFreqTxt.Location = new System.Drawing.Point(83, 25);
            this.RefreshFreqTxt.Name = "RefreshFreqTxt";
            this.RefreshFreqTxt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RefreshFreqTxt.Size = new System.Drawing.Size(79, 21);
            this.RefreshFreqTxt.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "检查频率(秒)";
            // 
            // AutoRestartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 452);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ShowTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "AutoRestartForm";
            this.Text = "重启监视工具";
            this.Load += new System.EventHandler(this.AutoRestartForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ShowTable)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除选中行的数据ToolStripMenuItem;
        private System.Windows.Forms.DataGridView ShowTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreateTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContentColumn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button RestartBtn;
        private System.Windows.Forms.Button RefreshTableBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label CurStateLbl;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox RefreshFreqTxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox MaxMissCountTxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.TextBox LogShowCountTxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label RestartCountLbl;
        private System.Windows.Forms.Label label5;
    }
}