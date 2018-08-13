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

namespace CQPMonitor.Tools.SysConfiger
{
    public partial class ConfigForm : Form
    {
        private AIConfig config { get; set; }

        public ConfigForm(AIConfig config)
        {
            InitializeComponent();

            this.config = config;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            if (config != null)
            {
                keyTxt.Text = config.Key;
                valueTxt.Text = config.Value;
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (config == null)
            {
                InsertConfig();
            }
            else
            {
                ModifyConfig();
            }

            this.Close();
        }

        private void InsertConfig()
        {
            using (AIDatabase db = new AIDatabase())
            {
                db.AIConfig.Add(new AIConfig
                {
                    Id = Guid.NewGuid().ToString(),
                    Group = "",
                    Key = keyTxt.Text,
                    Value = valueTxt.Text
                });

                db.SaveChanges();
            }
        }

        private void ModifyConfig()
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.AIConfig.First(p => p.Id == config.Id);
                query.Key = keyTxt.Text;
                query.Value = valueTxt.Text;

                db.SaveChanges();
            }
        }
    }
}