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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ShowTable = new System.Windows.Forms.DataGridView();
            this.CreateTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除选中行的数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.CurStateLbl = new System.Windows.Forms.Label();
            this.RestartBtn = new System.Windows.Forms.Button();
            this.RefreshTableBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ShowTable)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.5F));
            this.tableLayoutPanel1.Controls.Add(this.ShowTable, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ShowTable
            // 
            this.ShowTable.AllowUserToAddRows = false;
            this.ShowTable.AllowUserToDeleteRows = false;
            this.ShowTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ShowTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CreateTimeColumn,
            this.TypeColumn,
            this.ContentColumn});
            this.ShowTable.ContextMenuStrip = this.contextMenuStrip1;
            this.ShowTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShowTable.Location = new System.Drawing.Point(367, 3);
            this.ShowTable.Name = "ShowTable";
            this.ShowTable.ReadOnly = true;
            this.ShowTable.RowHeadersVisible = false;
            this.ShowTable.RowTemplate.Height = 23;
            this.ShowTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ShowTable.Size = new System.Drawing.Size(430, 444);
            this.ShowTable.TabIndex = 0;
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
            this.TypeColumn.DataPropertyName = "Type";
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
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.CurStateLbl, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.RestartBtn, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.RefreshTableBtn, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36.36364F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 63.63636F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 374F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(358, 444);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "当前状态：";
            // 
            // CurStateLbl
            // 
            this.CurStateLbl.AutoSize = true;
            this.CurStateLbl.Location = new System.Drawing.Point(183, 16);
            this.CurStateLbl.Name = "CurStateLbl";
            this.CurStateLbl.Size = new System.Drawing.Size(29, 12);
            this.CurStateLbl.TabIndex = 1;
            this.CurStateLbl.Text = "正常";
            // 
            // RestartBtn
            // 
            this.RestartBtn.Location = new System.Drawing.Point(6, 39);
            this.RestartBtn.Name = "RestartBtn";
            this.RestartBtn.Size = new System.Drawing.Size(75, 21);
            this.RestartBtn.TabIndex = 2;
            this.RestartBtn.Text = "重启";
            this.RestartBtn.UseVisualStyleBackColor = true;
            this.RestartBtn.Click += new System.EventHandler(this.RestartBtn_Click);
            // 
            // RefreshTableBtn
            // 
            this.RefreshTableBtn.Location = new System.Drawing.Point(183, 39);
            this.RefreshTableBtn.Name = "RefreshTableBtn";
            this.RefreshTableBtn.Size = new System.Drawing.Size(75, 21);
            this.RefreshTableBtn.TabIndex = 3;
            this.RefreshTableBtn.Text = "刷新";
            this.RefreshTableBtn.UseVisualStyleBackColor = true;
            this.RefreshTableBtn.Click += new System.EventHandler(this.RefreshTableBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(6, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 36);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "自动重启";
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
            // AutoRestartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AutoRestartForm";
            this.Text = "重启监视工具";
            this.Load += new System.EventHandler(this.AutoRestartForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ShowTable)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView ShowTable;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label CurStateLbl;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除选中行的数据ToolStripMenuItem;
        private System.Windows.Forms.Button RestartBtn;
        private System.Windows.Forms.Button RefreshTableBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreateTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContentColumn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}