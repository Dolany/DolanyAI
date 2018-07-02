namespace StarFortune
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
            this.starCombo = new System.Windows.Forms.ComboBox();
            this.queryBtn = new System.Windows.Forms.Button();
            this.showTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // starCombo
            // 
            this.starCombo.FormattingEnabled = true;
            this.starCombo.Items.AddRange(new object[] {
            "白羊座",
            "金牛座",
            "双子座",
            "巨蟹座",
            "狮子座",
            "处女座",
            "天秤座",
            "射手座",
            "摩羯座",
            "水瓶座",
            "双鱼座"});
            this.starCombo.Location = new System.Drawing.Point(12, 12);
            this.starCombo.Name = "starCombo";
            this.starCombo.Size = new System.Drawing.Size(121, 20);
            this.starCombo.TabIndex = 0;
            this.starCombo.Text = "白羊座";
            // 
            // queryBtn
            // 
            this.queryBtn.Location = new System.Drawing.Point(139, 10);
            this.queryBtn.Name = "queryBtn";
            this.queryBtn.Size = new System.Drawing.Size(75, 23);
            this.queryBtn.TabIndex = 1;
            this.queryBtn.Text = "查询";
            this.queryBtn.UseVisualStyleBackColor = true;
            this.queryBtn.Click += new System.EventHandler(this.queryBtn_Click);
            // 
            // showTxt
            // 
            this.showTxt.Location = new System.Drawing.Point(12, 38);
            this.showTxt.Multiline = true;
            this.showTxt.Name = "showTxt";
            this.showTxt.Size = new System.Drawing.Size(202, 343);
            this.showTxt.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 393);
            this.Controls.Add(this.showTxt);
            this.Controls.Add(this.queryBtn);
            this.Controls.Add(this.starCombo);
            this.Name = "Form1";
            this.Text = "星座运势查询";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox starCombo;
        private System.Windows.Forms.Button queryBtn;
        private System.Windows.Forms.TextBox showTxt;
    }
}

