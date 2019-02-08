namespace Dolany.Test.Information
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.GroupNumTxt = new System.Windows.Forms.TextBox();
            this.QQNumTxt = new System.Windows.Forms.TextBox();
            this.MsgTxt = new System.Windows.Forms.TextBox();
            this.SendBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "GroupNum";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "QQNum";
            // 
            // GroupNumTxt
            // 
            this.GroupNumTxt.Location = new System.Drawing.Point(112, 25);
            this.GroupNumTxt.Name = "GroupNumTxt";
            this.GroupNumTxt.Size = new System.Drawing.Size(131, 21);
            this.GroupNumTxt.TabIndex = 2;
            this.GroupNumTxt.Text = "411277569";
            // 
            // QQNumTxt
            // 
            this.QQNumTxt.Location = new System.Drawing.Point(112, 82);
            this.QQNumTxt.Name = "QQNumTxt";
            this.QQNumTxt.Size = new System.Drawing.Size(131, 21);
            this.QQNumTxt.TabIndex = 3;
            this.QQNumTxt.Text = "1458978159";
            // 
            // MsgTxt
            // 
            this.MsgTxt.Location = new System.Drawing.Point(36, 127);
            this.MsgTxt.Multiline = true;
            this.MsgTxt.Name = "MsgTxt";
            this.MsgTxt.Size = new System.Drawing.Size(207, 171);
            this.MsgTxt.TabIndex = 8;
            // 
            // SendBtn
            // 
            this.SendBtn.Location = new System.Drawing.Point(96, 316);
            this.SendBtn.Name = "SendBtn";
            this.SendBtn.Size = new System.Drawing.Size(75, 23);
            this.SendBtn.TabIndex = 9;
            this.SendBtn.Text = "发送";
            this.SendBtn.UseVisualStyleBackColor = true;
            this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 363);
            this.Controls.Add(this.SendBtn);
            this.Controls.Add(this.MsgTxt);
            this.Controls.Add(this.QQNumTxt);
            this.Controls.Add(this.GroupNumTxt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Inforamtion模拟";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox GroupNumTxt;
        private System.Windows.Forms.TextBox QQNumTxt;
        private System.Windows.Forms.TextBox MsgTxt;
        private System.Windows.Forms.Button SendBtn;
    }
}

