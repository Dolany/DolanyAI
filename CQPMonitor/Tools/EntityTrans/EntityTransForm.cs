using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            //var entities = DbMgr.Query<AlertContentEntity>();
            //foreach (var en in entities)
            //{
            //    using (AIDatabase db = new AIDatabase())
            //    {
            //        db.AlertContent.Add(new AILib.Db.AlertContent
            //        {
            //            Id = en.Id,
            //            Content = en.Content,
            //            FromGroup = en.FromGroup,
            //            AimHour = en.AimHour,
            //            CreateTime = en.CreateTime,
            //            Creator = en.Creator
            //        });

            //        db.SaveChanges();
            //    }
            //}

            //MessageBox.Show("complete " + entities.Count());
        }
    }
}