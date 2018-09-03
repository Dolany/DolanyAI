namespace KanColeSingleCli
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
            this.UrlTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NameTxt = new System.Windows.Forms.TextBox();
            this.LogTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AimTxt = new System.Windows.Forms.TextBox();
            this.StartBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UrlTxt
            // 
            this.UrlTxt.Location = new System.Drawing.Point(82, 12);
            this.UrlTxt.Name = "UrlTxt";
            this.UrlTxt.Size = new System.Drawing.Size(404, 21);
            this.UrlTxt.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Url";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name";
            // 
            // NameTxt
            // 
            this.NameTxt.Location = new System.Drawing.Point(82, 45);
            this.NameTxt.Name = "NameTxt";
            this.NameTxt.Size = new System.Drawing.Size(404, 21);
            this.NameTxt.TabIndex = 3;
            // 
            // LogTxt
            // 
            this.LogTxt.Location = new System.Drawing.Point(23, 152);
            this.LogTxt.Multiline = true;
            this.LogTxt.Name = "LogTxt";
            this.LogTxt.ReadOnly = true;
            this.LogTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTxt.Size = new System.Drawing.Size(463, 261);
            this.LogTxt.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Aim";
            // 
            // AimTxt
            // 
            this.AimTxt.Location = new System.Drawing.Point(82, 78);
            this.AimTxt.Name = "AimTxt";
            this.AimTxt.Size = new System.Drawing.Size(404, 21);
            this.AimTxt.TabIndex = 6;
            this.AimTxt.Text = "C:\\Users\\dolan\\Desktop\\少女前线\\";
            // 
            // StartBtn
            // 
            this.StartBtn.Location = new System.Drawing.Point(218, 113);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(75, 23);
            this.StartBtn.TabIndex = 7;
            this.StartBtn.Text = "开始";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 432);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.AimTxt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LogTxt);
            this.Controls.Add(this.NameTxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UrlTxt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UrlTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NameTxt;
        private System.Windows.Forms.TextBox LogTxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AimTxt;
        private System.Windows.Forms.Button StartBtn;
    }
}

