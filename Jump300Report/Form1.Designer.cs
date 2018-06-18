namespace Jump300Report
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.nameTxt = new System.Windows.Forms.TextBox();
            this.QueryBtn = new System.Windows.Forms.Button();
            this.showTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // nameTxt
            // 
            this.nameTxt.Location = new System.Drawing.Point(26, 12);
            this.nameTxt.Name = "nameTxt";
            this.nameTxt.Size = new System.Drawing.Size(201, 21);
            this.nameTxt.TabIndex = 0;
            this.nameTxt.Text = "苍月风语";
            // 
            // QueryBtn
            // 
            this.QueryBtn.Location = new System.Drawing.Point(249, 10);
            this.QueryBtn.Name = "QueryBtn";
            this.QueryBtn.Size = new System.Drawing.Size(79, 23);
            this.QueryBtn.TabIndex = 1;
            this.QueryBtn.Text = "查询";
            this.QueryBtn.UseVisualStyleBackColor = true;
            this.QueryBtn.Click += new System.EventHandler(this.QueryBtn_Click);
            // 
            // showTxt
            // 
            this.showTxt.Location = new System.Drawing.Point(26, 54);
            this.showTxt.Multiline = true;
            this.showTxt.Name = "showTxt";
            this.showTxt.ReadOnly = true;
            this.showTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.showTxt.Size = new System.Drawing.Size(302, 339);
            this.showTxt.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 405);
            this.Controls.Add(this.showTxt);
            this.Controls.Add(this.QueryBtn);
            this.Controls.Add(this.nameTxt);
            this.Name = "Form1";
            this.Text = "战绩查询";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameTxt;
        private System.Windows.Forms.Button QueryBtn;
        private System.Windows.Forms.TextBox showTxt;
    }
}

