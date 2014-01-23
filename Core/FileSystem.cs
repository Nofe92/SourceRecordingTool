using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public static class FileSystem
    {
        public static void OpenDirectory(string path)
        {
            Open("explorer.exe", path);
        }

        public static void OpenExplorer(string path)
        {
            Open("explorer.exe", "/select,\"" + path + "\"");
        }

        public static void Open(string fileName)
        {
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(fileName);
                process.Start();
            }
        }

        public static void Open(string fileName, string arguments)
        {
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo(fileName, arguments);
                process.Start();
            }
        }
    }
}
