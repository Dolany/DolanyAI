using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AILib;
using AILib.Entities;
using AILib.Db;

namespace CQPMonitor.Tools.BlackList
{
    [Tool(
        ToolName = "黑名单管理",
        Decription = "管理黑名单以及屏蔽词",
        ToolIcon = "blacklist.ico",
        IsAutoStart = false,
        Order = 3
        )]
    public partial class BlackListForm : ToolBaseForm
    {
        private List<AILib.Db.BlackList> BlackListList;
        private List<DirtyWord> DirtyWordList;

        public BlackListForm()
        {
            InitializeComponent();
        }

        private void RefreshBlackListTable()
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.BlackList.OrderByDescending(b => b.BlackCount);
                BlackListList = query.ToList();
            }
            blackListTable.DataSource = BlackListList;
        }

        private void RefreshDirtyWordsTable()
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.DirtyWord;
                DirtyWordList = query.ToList();
            }
            dirtyWordsTable.DataSource = DirtyWordList;
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            RefreshBlackListTable();
        }

        private void BlackListForm_Load(object sender, EventArgs e)
        {
            RefreshBlackListTable();
            RefreshDirtyWordsTable();
        }

        private void TimeUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            RefreshBlackListTable();
        }

        private void NewDiretyWordBtn_Click(object sender, EventArgs e)
        {
            NewDirtyWordForm f = new NewDirtyWordForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                RefreshDirtyWordsTable();
            }
        }

        private void 封印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var curRow = blackListTable.CurrentRow;
            if (curRow == null || curRow.Index < 0)
            {
                return;
            }

            var id = BlackListList[curRow.Index].Id;
            using (AIDatabase db = new AIDatabase())
            {
                var entity = db.BlackList.First(b => b.Id == id);
                entity.BlackCount = 10;
                db.SaveChanges();
            }

            RefreshBlackListTable();
        }

        private void 重置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var curRow = blackListTable.CurrentRow;
            if (curRow == null || curRow.Index < 0)
            {
                return;
            }

            var id = BlackListList[curRow.Index].Id;
            using (AIDatabase db = new AIDatabase())
            {
                var entity = db.BlackList.First(b => b.Id == id);
                entity.BlackCount = 0;
                db.SaveChanges();
            }

            RefreshBlackListTable();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var curRow = dirtyWordsTable.CurrentRow;
            if (curRow == null || curRow.Index < 0)
            {
                return;
            }

            using (AIDatabase db = new AIDatabase())
            {
                var id = DirtyWordList[curRow.Index].Id;
                var query = db.DirtyWord.Where(d => d.Id == id);
                db.DirtyWord.RemoveRange(query);
                db.SaveChanges();
            }
            RefreshDirtyWordsTable();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}