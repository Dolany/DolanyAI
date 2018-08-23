namespace AIMonitor.Tools.SysConfiger
{
    partial class SysConfigerForm
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
            this.configTable = new System.Windows.Forms.DataGridView();
            this.addBtn = new System.Windows.Forms.Button();
            this.refreshBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.configTable)).BeginInit();
            this.SuspendLayout();
            // 
            // configTable
            // 
            this.configTable.AllowUserToAddRows = false;
            this.configTable.AllowUserToDeleteRows = false;
            this.configTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.configTable.Location = new System.Drawing.Point(12, 12);
            this.configTable.Name = "configTable";
            this.configTable.ReadOnly = true;
            this.configTable.RowTemplate.Height = 23;
            this.configTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.configTable.Size = new System.Drawing.Size(283, 402);
            this.configTable.TabIndex = 0;
            this.configTable.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.configTable_CellDoubleClick);
            // 
            // addBtn
            // 
            this.addBtn.Location = new System.Drawing.Point(27, 420);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(75, 23);
            this.addBtn.TabIndex = 1;
            this.addBtn.Text = "添加";
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // refreshBtn
            // 
            this.refreshBtn.Location = new System.Drawing.Point(207, 420);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(75, 23);
            this.refreshBtn.TabIndex = 2;
            this.refreshBtn.Text = "刷新";
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // SysConfigerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 450);
            this.Controls.Add(this.refreshBtn);
            this.Controls.Add(this.addBtn);
            this.Controls.Add(this.configTable);
            this.Name = "SysConfigerForm";
            this.Text = "系统配置";
            this.Load += new System.EventHandler(this.SysConfigerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.configTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView configTable;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.Button refreshBtn;
    }
}