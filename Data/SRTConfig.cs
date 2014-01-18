using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public static class SRTConfig
    {
        private static ComboBox configComboBox;
        private static FileSystemWatcher cfgFileSystemWatcher;

        internal static void Initialize(MainForm mainForm)
        {
            configComboBox = mainForm.ConfigComboBox;

            Directory.CreateDirectory("moviefiles\\cfg");

            foreach (string file in Directory.EnumerateFiles("moviefiles\\cfg", "*.cfg"))
                configComboBox.Items.Add(Path.GetFileName(file));

            cfgFileSystemWatcher = new FileSystemWatcher("moviefiles\\cfg", "*.cfg");
            cfgFileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
            cfgFileSystemWatcher.SynchronizingObject = mainForm;
            cfgFileSystemWatcher.Created += new FileSystemEventHandler(cfgFileSystemWatcher_Created);
            cfgFileSystemWatcher.Deleted += new FileSystemEventHandler(cfgFileSystemWatcher_Deleted);
            cfgFileSystemWatcher.Renamed += new RenamedEventHandler(cfgFileSystemWatcher_Renamed);

            cfgFileSystemWatcher.EnableRaisingEvents = true;
        }

        private static void cfgFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            configComboBox.Items.Add(e.Name);
        }

        private static void cfgFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            int index = configComboBox.Items.IndexOf(e.OldName);

            if (index == -1)
                configComboBox.Items.Add(e.Name);
            else if (e.Name.EndsWith(".cfg"))
                configComboBox.Items[index] = e.Name;
            else
                configComboBox.Items.RemoveAt(index);

            if (configComboBox.SelectedIndex == -1)
                configComboBox.SelectedIndex = 0;
        }

        private static void cfgFileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            configComboBox.Items.Remove(e.Name);

            if (configComboBox.SelectedIndex == -1)
                configComboBox.SelectedIndex = 0;
        }
    }
}
