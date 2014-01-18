using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public static class SRTCustom
    {
        private static CheckedListBox customCheckedListBox;
        private static FileSystemWatcher customFileSystemWatcher;
        private static FileSystemWatcher customDisabledFileSystemWatcher;
        private static ContextMenuStrip contextMenuStrip;
        private static ToolStripMenuItem view;
        private static ToolStripMenuItem viewContents;
        private static ToolStripSeparator separator;
        private static ToolStripMenuItem checkAll;
        private static ToolStripMenuItem checkNone;
        private static bool disableCustomEvents = false;

        internal static void Initialize(MainForm mainForm)
        {
            customCheckedListBox = mainForm.CustomCheckedListBox;

            Directory.CreateDirectory("moviefiles\\custom");
            Directory.CreateDirectory("moviefiles\\custom_disabled");

            foreach (string file in Directory.EnumerateFileSystemEntries("moviefiles\\custom"))
                mainForm.CustomCheckedListBox.Items.Add(Path.GetFileName(file), true);

            foreach (string file in Directory.EnumerateFileSystemEntries("moviefiles\\custom_disabled"))
                mainForm.CustomCheckedListBox.Items.Add(Path.GetFileName(file), false);

            customFileSystemWatcher = new FileSystemWatcher("moviefiles\\custom");
            customFileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            customFileSystemWatcher.SynchronizingObject = mainForm;
            customFileSystemWatcher.Created += new FileSystemEventHandler(customFileSystemWatcher_Created);
            customFileSystemWatcher.Deleted += new FileSystemEventHandler(customFileSystemWatcher_Deleted);
            customFileSystemWatcher.Renamed += new RenamedEventHandler(customFileSystemWatcher_Renamed);

            customDisabledFileSystemWatcher = new FileSystemWatcher("moviefiles\\custom_disabled");
            customDisabledFileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            customDisabledFileSystemWatcher.SynchronizingObject = mainForm;
            customDisabledFileSystemWatcher.Created += new FileSystemEventHandler(customFileSystemWatcher_Created);
            customDisabledFileSystemWatcher.Deleted += new FileSystemEventHandler(customFileSystemWatcher_Deleted);
            customDisabledFileSystemWatcher.Renamed += new RenamedEventHandler(customFileSystemWatcher_Renamed);

            customFileSystemWatcher.EnableRaisingEvents = true;
            customDisabledFileSystemWatcher.EnableRaisingEvents = true;

            contextMenuStrip = new ContextMenuStrip();
            view = (ToolStripMenuItem)contextMenuStrip.Items.Add("View");
            viewContents = (ToolStripMenuItem)contextMenuStrip.Items.Add("View Contents");
            separator = (ToolStripSeparator)contextMenuStrip.Items[contextMenuStrip.Items.Add(new ToolStripSeparator())];
            checkAll = (ToolStripMenuItem)contextMenuStrip.Items.Add("Check All");
            checkNone = (ToolStripMenuItem)contextMenuStrip.Items.Add("Check None");
            
            contextMenuStrip.Opened += contextMenuStrip_Opened;
            view.Click += view_Click;
            viewContents.Click += viewContents_Click;
            checkAll.Click += checkAll_Click;
            checkNone.Click += checkNone_Click;
            customCheckedListBox.ContextMenuStrip = contextMenuStrip;

            customCheckedListBox.MouseDown += customCheckedListBox_MouseDown;
            customCheckedListBox.ItemCheck += customCheckedListBox_ItemCheck;
        }

        private static void view_Click(object sender, EventArgs e)
        {
            if (customCheckedListBox.GetItemChecked(customCheckedListBox.SelectedIndex))
                FileSystem.Open("explorer.exe", String.Concat("/select,moviefiles\\custom\\", (string)customCheckedListBox.SelectedItem));
            else
                FileSystem.Open("explorer.exe", String.Concat("/select,moviefiles\\custom_disabled\\", (string)customCheckedListBox.SelectedItem));
        }

        private static void contextMenuStrip_Opened(object sender, EventArgs e)
        {
            bool itemsSelected = customCheckedListBox.SelectedIndex != -1;

            view.Visible = itemsSelected;
            viewContents.Visible = itemsSelected;
            separator.Visible = itemsSelected;
        }

        private static void viewContents_Click(object sender, EventArgs e)
        {
            if (customCheckedListBox.GetItemChecked(customCheckedListBox.SelectedIndex))
                FileSystem.Open(String.Concat("moviefiles\\custom\\", (string)customCheckedListBox.SelectedItem));
            else
                FileSystem.Open(String.Concat("moviefiles\\custom_disabled\\", (string)customCheckedListBox.SelectedItem));
        }

        private static void checkAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < customCheckedListBox.Items.Count; i++)
                customCheckedListBox.SetItemChecked(i, true);
        }

        private static void checkNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < customCheckedListBox.Items.Count; i++)
                customCheckedListBox.SetItemChecked(i, false);
        }

        private static void customCheckedListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                customCheckedListBox.SelectedIndex = customCheckedListBox.IndexFromPoint(e.Location);
        }

        private static void customCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (disableCustomEvents)
                return;

            if ((e.NewValue = e.CurrentValue) == CheckState.Checked)
            {
                if (File.Exists("moviefiles\\custom\\" + customCheckedListBox.Items[e.Index]))
                    File.Move("moviefiles\\custom\\" + customCheckedListBox.Items[e.Index], "moviefiles\\custom_disabled\\" + customCheckedListBox.Items[e.Index]);
                else if (Directory.Exists("moviefiles\\custom\\" + customCheckedListBox.Items[e.Index]))
                    Directory.Move("moviefiles\\custom\\" + customCheckedListBox.Items[e.Index], "moviefiles\\custom_disabled\\" + customCheckedListBox.Items[e.Index]);
            }
            else
            {
                if (File.Exists("moviefiles\\custom_disabled\\" + customCheckedListBox.Items[e.Index]))
                    File.Move("moviefiles\\custom_disabled\\" + customCheckedListBox.Items[e.Index], "moviefiles\\custom\\" + customCheckedListBox.Items[e.Index]);
                else if (Directory.Exists("moviefiles\\custom_disabled\\" + customCheckedListBox.Items[e.Index]))
                    Directory.Move("moviefiles\\custom_disabled\\" + customCheckedListBox.Items[e.Index], "moviefiles\\custom\\" + customCheckedListBox.Items[e.Index]);
            }
        }

        private static void customFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            disableCustomEvents = true;
            int index = customCheckedListBox.Items.IndexOf(e.Name);

            if (index == -1)
                customCheckedListBox.Items.Add(e.Name, sender == customFileSystemWatcher);
            else
                customCheckedListBox.SetItemChecked(index, sender == customFileSystemWatcher);
            disableCustomEvents = false;
        }

        private static void customFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            disableCustomEvents = true;
            int index = customCheckedListBox.Items.IndexOf(e.OldName);

            if (index != -1)
                customCheckedListBox.Items[index] = e.Name;
            disableCustomEvents = false;
        }

        private static void customFileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            disableCustomEvents = true;
            if (!Directory.Exists(String.Concat("moviefiles\\", sender == customFileSystemWatcher ? "custom_disabled\\" : "custom\\", e.Name)) && !File.Exists(String.Concat("moviefiles\\", sender == customFileSystemWatcher ? "custom_disabled\\" : "custom\\", e.Name)))
                customCheckedListBox.Items.Remove(e.Name);
            disableCustomEvents = false;
        }
    }
}
