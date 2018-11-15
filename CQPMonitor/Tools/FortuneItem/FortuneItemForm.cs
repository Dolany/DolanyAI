﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dolany.Ai.Reborn.DolanyAI.Db;

namespace AIMonitor.Tools.FortuneItem
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
        private List<Dolany.Ai.Reborn.DolanyAI.Db.FortuneItem> fortuneItems;

        public FortuneItemForm()
        {
            InitializeComponent();
        }

        private void dataTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            using (var fief = new FortuneItemEditForm(fortuneItems[e.RowIndex]))
            {
                fief.ShowDialog();
                RefreshTable();
            }
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
            using (var fief = new FortuneItemEditForm(null))
            {
                fief.ShowDialog();
                RefreshTable();
            }
        }
    }
}