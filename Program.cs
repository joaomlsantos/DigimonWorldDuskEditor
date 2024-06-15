using System;
using System.Windows.Forms;
using DigimonWorldDuskEditor.Forms;

namespace DigimonWorldDuskEditor
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}