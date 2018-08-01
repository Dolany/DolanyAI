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
using Dolany.Ice.Ai.DolanyAI;

namespace CQPMonitor.Tools
{
    [Tool(
        ToolName = "Entity转换",
        Decription = "Entity转换",
        ToolIcon = "",
        IsAutoStart = true,
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
            //var entities = DbMgr.Query<SayingEntity>();
            //foreach (var en in entities)
            //{
            //    using (AIDatabase db = new AIDatabase())
            //    {
            //        db.Saying.Add(new Saying
            //        {
            //            Id = en.Id,
            //            Content = en.Content,
            //            FromGroup = en.FromGroup,
            //            Cartoon = en.Cartoon,
            //            Charactor = en.Charactor
            //        });

            //        db.SaveChanges();
            //    }
            //}

            //MessageBox.Show("complete " + entities.Count());
        }
    }
}