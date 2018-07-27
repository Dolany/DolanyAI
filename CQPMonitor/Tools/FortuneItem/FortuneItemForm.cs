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
    [Tool(
        ToolName = "运势加成物品",
        Decription = "编辑运势加成物品",
        ToolIcon = "fortuneitem.ico",
        IsAutoStart = true,
        Order = 6
        )]
    public partial class FortuneItemForm : ToolBaseForm
    {
        private List<Dolany.Ice.Ai.DolanyAI.Db.FortuneItem> fortuneItems;

        public FortuneItemForm()
        {
            InitializeComponent();
        }

        private void dataTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FortuneItemEditForm fief = new FortuneItemEditForm(fortuneItems[e.RowIndex]);
            fief.ShowDialog();
            RefreshTable();
        }

        private void dataTable_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                RefreshTable();
                return;
            }

            using (AIDatabase db = new AIDatabase())
            {
                db.FortuneItem.Remove(fortuneItems[e.RowIndex]);
                db.SaveChanges();
            }
        }

        private void FortuneItemForm_Load(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void RefreshTable()
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.FortuneItem;
                if (query.IsNullOrEmpty())
                {
                    return;
                }

                fortuneItems = query.ToList();
                dataTable.DataSource = fortuneItems;
            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            FortuneItemEditForm fief = new FortuneItemEditForm(null);
            fief.ShowDialog();
            RefreshTable();
        }
    }
}