﻿using System;

namespace AIMonitor.Tools.FishingEditor
{
    [Tool(
        ToolName = "钓鱼编辑器",
        Decription = "钓鱼编辑器",
        ToolIcon = "fishingeditor.ico",
        IsAutoStart = false,
        Order = 4
        )]
    public partial class FishingEditorForm : ToolBaseForm
    {
        public FishingEditorForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //using (AIDatabase db = new AIDatabase())
            //{
            //    var query = db.PlusOneAvailable.Where(r => r.GroupNumber == 411277569 && !r.Available);

            //    MessageBox.Show((!query.IsNullOrEmpty()).ToString());
            //}
        }
    }
}