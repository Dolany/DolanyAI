namespace CQPMonitor.Tools.BlackList
{
    partial class BlackListForm
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
            this.blackListTable = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.封印ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dirtyWordsTable = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.NewDiretyWordBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.blackListTable)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dirtyWordsTable)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // blackListTable
            // 
            this.blackListTable.AllowUserToAddRows = false;
            this.blackListTable.AllowUserToDeleteRows = false;
            this.blackListTable.AllowUserToResizeRows = false;
            this.blackListTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.blackListTable.ContextMenuStrip = this.contextMenuStrip1;
            this.blackListTable.Location = new System.Drawing.Point(3, 42);
            this.blackListTable.MultiSelect = false;
            this.blackListTable.Name = "blackListTable";
            this.blackListTable.ReadOnly = true;
            this.blackListTable.RowHeadersVisible = false;
            this.blackListTable.RowTemplate.Height = 23;
            this.blackListTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.blackListTable.Size = new System.Drawing.Size(277, 404);
            this.blackListTable.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.封印ToolStripMenuItem,
            this.重置ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // 封印ToolStripMenuItem
            // 
            this.封印ToolStripMenuItem.Name = "封印ToolStripMenuItem";
            this.封印ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.封印ToolStripMenuItem.Text = "封印";
            this.封印ToolStripMenuItem.Click += new System.EventHandler(this.封印ToolStripMenuItem_Click);
            // 
            // 重置ToolStripMenuItem
            // 
            this.重置ToolStripMenuItem.Name = "重置ToolStripMenuItem";
            this.重置ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.重置ToolStripMenuItem.Text = "重置";
            this.重置ToolStripMenuItem.Click += new System.EventHandler(this.重置ToolStripMenuItem_Click);
            // 
            // dirtyWordsTable
            // 
            this.dirtyWordsTable.AllowUserToAddRows = false;
            this.dirtyWordsTable.AllowUserToDeleteRows = false;
            this.dirtyWordsTable.AllowUserToResizeRows = false;
            this.dirtyWordsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dirtyWordsTable.ContextMenuStrip = this.contextMenuStrip2;
            this.dirtyWordsTable.Location = new System.Drawing.Point(300, 42);
            this.dirtyWordsTable.MultiSelect = false;
            this.dirtyWordsTable.Name = "dirtyWordsTable";
            this.dirtyWordsTable.ReadOnly = true;
            this.dirtyWordsTable.RowHeadersVisible = false;
            this.dirtyWordsTable.RowTemplate.Height = 23;
            this.dirtyWordsTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dirtyWordsTable.Size = new System.Drawing.Size(284, 404);
            this.dirtyWordsTable.TabIndex = 1;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除ToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(101, 26);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(98, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "黑名单";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(427, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "屏蔽词";
            // 
            // RefreshBtn
            // 
            this.RefreshBtn.Location = new System.Drawing.Point(89, 452);
            this.RefreshBtn.Name = "RefreshBtn";
            this.RefreshBtn.Size = new System.Drawing.Size(75, 23);
            this.RefreshBtn.TabIndex = 4;
            this.RefreshBtn.Text = "刷新";
            this.RefreshBtn.UseVisualStyleBackColor = true;
            this.RefreshBtn.Click += new System.EventHandler(this.RefreshBtn_Click);
            // 
            // NewDiretyWordBtn
            // 
            this.NewDiretyWordBtn.Location = new System.Drawing.Point(405, 452);
            this.NewDiretyWordBtn.Name = "NewDiretyWordBtn";
            this.NewDiretyWordBtn.Size = new System.Drawing.Size(75, 23);
            this.NewDiretyWordBtn.TabIndex = 5;
            this.NewDiretyWordBtn.Text = "新增";
            this.NewDiretyWordBtn.UseVisualStyleBackColor = true;
            this.NewDiretyWordBtn.Click += new System.EventHandler(this.NewDiretyWordBtn_Click);
            // 
            // BlackListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 482);
            this.Controls.Add(this.NewDiretyWordBtn);
            this.Controls.Add(this.RefreshBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dirtyWordsTable);
            this.Controls.Add(this.blackListTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "BlackListForm";
            this.Text = "黑名单管理";
            this.Load += new System.EventHandler(this.BlackListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.blackListTable)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dirtyWordsTable)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView blackListTable;
        private System.Windows.Forms.DataGridView dirtyWordsTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button RefreshBtn;
        private System.Windows.Forms.Button NewDiretyWordBtn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 封印ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重置ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
    }
}