using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace CQPMonitor.Tools.BlackList
{
    public partial class NewDirtyWordForm : Form
    {
        public NewDirtyWordForm()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var word = wordTxt.Text;
            if (string.IsNullOrEmpty(word))
            {
                MessageBox.Show("屏蔽词不能为空！");
                wordTxt.Focus();
                return;
            }

            using (AIDatabase db = new AIDatabase())
            {
                var query = db.DirtyWord.Where(d => d.Content == word);
                if (!query.IsNullOrEmpty())
                {
                    MessageBox.Show("屏蔽词已存在！");
                    wordTxt.Focus();
                    return;
                }

                db.DirtyWord.Add(new DirtyWord
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = word
                });
                db.SaveChanges();
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}