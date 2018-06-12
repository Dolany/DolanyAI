using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace XMLIdAppender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "请选择文件";
                openFileDialog.Filter = "xml文件(*.xml;)|*.xml";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string file = openFileDialog.FileName;
                    AppendIdToXml(file);
                    MessageBox.Show("Append Success!");
                }
            }
            catch
            {
                MessageBox.Show("Append Failed!");
            }
        }

        private void AppendIdToXml(string fileName)
        {
            XElement root = XElement.Load(fileName);
            foreach (XElement node in root.Nodes())
            {
                var attr = node.Attribute("Id");
                if (attr == null || string.IsNullOrEmpty(attr.Value))
                {
                    node.SetAttributeValue("Id", Guid.NewGuid().ToString());
                }
            }

            root.Save(fileName);
        }
    }
}
