using System;
using System.Windows.Forms;

namespace KanColeVoiceClimber
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var form1 = new Form1())
            {
                Application.Run(form1);
            }
        }
    }
}