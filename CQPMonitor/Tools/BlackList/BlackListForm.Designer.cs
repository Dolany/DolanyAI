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
            this.blackListTable = new System.Windows.Forms.DataGridView();
            this.dirtyWordsTable = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.NewDiretyWordBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.blackListTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dirtyWordsTable)).BeginInit();
            this.SuspendLayout();
            // 
            // blackListTable
            // 
            this.blackListTable.AllowUserToAddRows = false;
            this.blackListTable.AllowUserToOrderColumns = true;
            this.blackListTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.blackListTable.Location = new System.Drawing.Point(3, 42);
            this.blackListTable.Name = "blackListTable";
            this.blackListTable.RowTemplate.Height = 23;
            this.blackListTable.Size = new System.Drawing.Size(277, 404);
            this.blackListTable.TabIndex = 0;
            // 
            // dirtyWordsTable
            // 
            this.dirtyWordsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dirtyWordsTable.Location = new System.Drawing.Point(300, 42);
            this.dirtyWordsTable.Name = "dirtyWordsTable";
            this.dirtyWordsTable.RowTemplate.Height = 23;
            this.dirtyWordsTable.Size = new System.Drawing.Size(284, 404);
            this.dirtyWordsTable.TabIndex = 1;
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
            this.Name = "BlackListForm";
            this.Text = "黑名单管理";
            this.Load += new System.EventHandler(this.BlackListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.blackListTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dirtyWordsTable)).EndInit();
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
    }
}