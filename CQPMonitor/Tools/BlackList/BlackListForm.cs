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
        private List<BlackListEntity> BlackListList;
        private List<DirtyWordEntity> DirtyWordList;

        public BlackListForm()
        {
            InitializeComponent();
        }

        private void RefreshBlackListTable()
        {
            var query = DbMgr.Query<BlackListEntity>().OrderByDescending(b => b.UpdateTime);
            BlackListList = query.ToList();
            blackListTable.DataSource = BlackListList;
        }

        private void RefreshDirtyWordsTable()
        {
            var query = DbMgr.Query<DirtyWordEntity>();
            DirtyWordList = query.ToList();
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

            var entity = BlackListList[curRow.Index];
            entity.BlackCount = 10;
            DbMgr.Update(entity);

            RefreshBlackListTable();
        }

        private void 重置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var curRow = blackListTable.CurrentRow;
            if (curRow == null || curRow.Index < 0)
            {
                return;
            }

            var entity = BlackListList[curRow.Index];
            entity.BlackCount = 0;
            DbMgr.Update(entity);

            RefreshBlackListTable();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var curRow = dirtyWordsTable.CurrentRow;
            if (curRow == null || curRow.Index < 0)
            {
                return;
            }

            DbMgr.Delete<DirtyWordEntity>(DirtyWordList[curRow.Index].Id);
            RefreshDirtyWordsTable();
        }
    }
}