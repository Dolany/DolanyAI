using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AILib.Db;
using AILib.Entities;
using AILib;

namespace CQPMonitor.Tools
{
    [Tool(
        ToolName = "Entity转换",
        Decription = "Entity转换",
        ToolIcon = "",
        IsAutoStart = false,
        Order = 5
        )]
    public partial class EntityTransForm : ToolBaseForm
    {
        public EntityTransForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var entities = DbMgr.Query<RepeaterAvailableEntity>();
            //foreach (var en in entities)
            //{
            //    UpdateToSqlServer(en);
            //}

            //MessageBox.Show("complete " + entities.Count());
        }

        //private void UpdateToSqlServer(RepeaterAvailableEntity en)
        //{
        //    using (AIDatabase db = new AIDatabase())
        //    {
        //        db.RepeaterAvailable.Add(new RepeaterAvailable
        //        {
        //            Id = en.Id,
        //            GroupNumber = en.GroupNumber,
        //            Available = bool.Parse(en.Content)
        //        });

        //        db.SaveChanges();
        //    }
        //}
    }
}