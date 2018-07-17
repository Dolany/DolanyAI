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
            //var entities = DbMgr.Query<AlermClockEntity>();
            //foreach (var en in entities)
            //{
            //    using (AIDatabase db = new AIDatabase())
            //    {
            //        db.AlermClock.Add(new AlermClock
            //        {
            //            Id = en.Id,
            //            GroupNumber = en.GroupNumber,
            //            Creator = en.Creator,
            //            CreateTime = en.CreateTime,
            //            AimHourt = en.AimHourt,
            //            AimMinute = en.AimMinute,
            //            Content = en.Content
            //        });

            //        db.SaveChanges();
            //    }
            //}

            //MessageBox.Show("complete " + entities.Count());
        }
    }
}