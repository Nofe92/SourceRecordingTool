using System;
using System.IO;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!Updater.Update())
                return;

            Application.Run(new MainForm());
        }
    }
}
