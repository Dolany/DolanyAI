namespace DolanyToolControl
{
    partial class dolanyToolCon
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.IconBox = new System.Windows.Forms.PictureBox();
            this.NameLbl = new System.Windows.Forms.Label();
            this.DescriptionLbl = new System.Windows.Forms.Label();
            this.StatusLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.IconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // IconBox
            // 
            this.IconBox.Location = new System.Drawing.Point(0, 19);
            this.IconBox.Name = "IconBox";
            this.IconBox.Size = new System.Drawing.Size(51, 50);
            this.IconBox.TabIndex = 0;
            this.IconBox.TabStop = false;
            // 
            // NameLbl
            // 
            this.NameLbl.AutoSize = true;
            this.NameLbl.Location = new System.Drawing.Point(3, 4);
            this.NameLbl.Name = "NameLbl";
            this.NameLbl.Size = new System.Drawing.Size(41, 12);
            this.NameLbl.TabIndex = 1;
            this.NameLbl.Text = "label1";
            // 
            // DescriptionLbl
            // 
            this.DescriptionLbl.Location = new System.Drawing.Point(57, 19);
            this.DescriptionLbl.Name = "DescriptionLbl";
            this.DescriptionLbl.Size = new System.Drawing.Size(59, 50);
            this.DescriptionLbl.TabIndex = 2;
            this.DescriptionLbl.Text = "label1";
            // 
            // StatusLbl
            // 
            this.StatusLbl.AutoSize = true;
            this.StatusLbl.Location = new System.Drawing.Point(66, 4);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(41, 12);
            this.StatusLbl.TabIndex = 3;
            this.StatusLbl.Text = "label1";
            // 
            // dolanyToolCon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StatusLbl);
            this.Controls.Add(this.DescriptionLbl);
            this.Controls.Add(this.NameLbl);
            this.Controls.Add(this.IconBox);
            this.Name = "dolanyToolCon";
            this.Size = new System.Drawing.Size(119, 69);
            ((System.ComponentModel.ISupportInitialize)(this.IconBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox IconBox;
        private System.Windows.Forms.Label NameLbl;
        private System.Windows.Forms.Label DescriptionLbl;
        private System.Windows.Forms.Label StatusLbl;
    }
}
