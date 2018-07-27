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

namespace CQPMonitor.Tools.FortuneItem
{
    public partial class FortuneItemEditForm : Form
    {
        private Dolany.Ice.Ai.DolanyAI.Db.FortuneItem fi;

        public FortuneItemEditForm(Dolany.Ice.Ai.DolanyAI.Db.FortuneItem fi)
        {
            this.fi = fi;

            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            fi = null;
            Close();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            using (AIDatabase db = new AIDatabase())
            {
                if (fi == null)
                {
                    fi = new Dolany.Ice.Ai.DolanyAI.Db.FortuneItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = nameTxt.Text,
                        Description = DescriptionTxt.Text,
                        Value = int.Parse(ValueTxt.Text),
                        Type = TypeCombo.Text == "百分比" ? 0 : 1
                    };
                    db.FortuneItem.Add(fi);
                }
                else
                {
                    fi = db.FortuneItem.First(f => f.Id == fi.Id);
                    fi.Name = nameTxt.Text;
                    fi.Description = DescriptionTxt.Text;
                    fi.Value = int.Parse(ValueTxt.Text);
                    fi.Type = TypeCombo.Text == "百分比" ? 0 : 1;
                }

                db.SaveChanges();
            }

            Close();
        }

        private void FortuneItemEditForm_Load(object sender, EventArgs e)
        {
            if (fi == null)
            {
                return;
            }

            nameTxt.Text = fi.Name;
            DescriptionTxt.Text = fi.Description;
            ValueTxt.Text = fi.Value.ToString();
            TypeCombo.Text = fi.Type == 0 ? "百分比" : "绝对值";
        }
    }
}