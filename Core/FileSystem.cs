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
        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            foreach (string sourceFile in Directory.EnumerateFiles(sourceDirName))
                File.Copy(sourceFile, String.Concat(destDirName, "\\", Path.GetFileName(sourceFile)));

            foreach (string sourceDir in Directory.EnumerateDirectories(sourceDirName))
                CopyDirectory(sourceDir, String.Concat(destDirName, "\\", Path.GetFileName(sourceDir)));
        }

        public static void DeleteDirectory(string path)
        {
            while (true)
            {
                try
                {
                    if (Directory.Exists(path))
                        Directory.Delete(path, true);
                    break;
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                        break;
                }
            }
        }

        public static void MoveDirectory(string sourceDirName, string destDirName)
        {
            while (true)
            {
                try
                {
                    if (Directory.Exists(sourceDirName))
                        Directory.Move(sourceDirName, destDirName);
                    break;
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                        break;
                }
            }
        }

        public static void OpenDirectory(string path)
        {
            if (Directory.Exists(path))
                Open("explorer.exe", path);
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
