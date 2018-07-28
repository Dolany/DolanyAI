namespace KanColeVoiceClimber
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
            this.ShowTxt = new System.Windows.Forms.TextBox();
            this.startBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ShowTxt
            // 
            this.ShowTxt.Location = new System.Drawing.Point(12, 12);
            this.ShowTxt.MaxLength = 999999999;
            this.ShowTxt.Multiline = true;
            this.ShowTxt.Name = "ShowTxt";
            this.ShowTxt.ReadOnly = true;
            this.ShowTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ShowTxt.Size = new System.Drawing.Size(177, 350);
            this.ShowTxt.TabIndex = 0;
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(58, 368);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 23);
            this.startBtn.TabIndex = 1;
            this.startBtn.Text = "开始";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(201, 397);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.ShowTxt);
            this.Name = "Form1";
            this.Text = "舰娘语音读取器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ShowTxt;
        private System.Windows.Forms.Button startBtn;
    }
}

