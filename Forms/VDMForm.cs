 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public partial class VDMForm : Form
    {
        public List<RecordingRange> RecordingRanges = new List<RecordingRange>();

        private List<DemoFile> demoFiles = new List<DemoFile>();
        private ListViewItem[] listViewItemCache;
        private List<ListViewItem> demoFilesFilter = new List<ListViewItem>();
        private RecordingRangeDialog recordingRangeDialog = new RecordingRangeDialog();
        private int lastColumnIndex = -1;
        
        public VDMForm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            Program.SetWindowTheme(demosListView.Handle, "explorer", null);
            Program.SetWindowTheme(rangesListView.Handle, "explorer", null);

            pathTextBox.Text = MainForm.CurrentProfile.Game.ShortNamePath;
            filterTextBox.Select();

            modeComboBox.SelectedIndex = MainForm.CurrentProfile.ScheduledRecordingMode;

            QueryDemos();
        }

        private void QueryDemos()
        {
            demoFiles.Clear();

            if (Directory.Exists(pathTextBox.Text))
            {
                foreach (string file in Directory.GetFiles(pathTextBox.Text, "*.dem"))
                {
                    using (BinaryReader demoReader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.ASCII))
                    {
                        if (new string(demoReader.ReadChars(8)) != "HL2DEMO\0")
                            continue;

                        DemoFile demo = new DemoFile();
                        demo.Name = Path.GetFileName(file);
                        demo.DirectoryName = pathTextBox.Text;
                        demo.DemoProtocol = demoReader.ReadInt32();
                        demo.NetworkProtocol = demoReader.ReadInt32();
                        demo.ServerName = new string(demoReader.ReadChars(260)).TrimEnd('\0');
                        demo.ClientName = new string(demoReader.ReadChars(260)).TrimEnd('\0');
                        demo.MapName = new string(demoReader.ReadChars(260)).TrimEnd('\0');
                        demo.GameDirectory = new string(demoReader.ReadChars(260)).TrimEnd('\0');
                        demo.PlaybackTime = demoReader.ReadSingle();
                        demo.Ticks = demoReader.ReadInt32();

                        demoFiles.Add(demo);
                    }
                }
            }

            listViewItemCache = new ListViewItem[demoFiles.Count];
            
            for (int i = 0; i < demoFiles.Count; i++)
            {
                DemoFile demo = demoFiles[i];

                listViewItemCache[i] = new ListViewItem(new string[]
                {
                    demo.Name,
                    demo.ServerName,
                    demo.ClientName,
                    demo.MapName,
                    demo.GameDirectory,
                    demo.PlaybackTime.ToString() + " s",
                    demo.Ticks.ToString(),
                    demo.DemoProtocol.ToString(),
                    demo.NetworkProtocol.ToString(),
                }) { Tag = demoFiles[i] };
            }

            RefreshDemos();
        }

        private void RefreshDemos()
        {
            demoFilesFilter.Clear();
            string match = filterTextBox.Text.ToLowerInvariant();

            for (int i = 0; i < demoFiles.Count; i++)
                if (
                    demoFiles[i].Name.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].ServerName.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].ClientName.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].MapName.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].GameDirectory.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].PlaybackTime.ToString().Contains(match) ||
                    demoFiles[i].Ticks.ToString().Contains(match) ||
                    demoFiles[i].DemoProtocol.ToString().Contains(match) ||
                    demoFiles[i].NetworkProtocol.ToString().Contains(match)
                    )
                    demoFilesFilter.Add(listViewItemCache[i]);

            demosListView.VirtualListSize = demoFilesFilter.Count;
            addRangeButton.Enabled = false;
        }

        private void AddRange()
        {
            DemoFile demo = (DemoFile)demoFilesFilter[demosListView.SelectedIndices[0]].Tag;

            recordingRangeDialog.Text = "Add Recording Range";
            recordingRangeDialog.demoNameTextBox.Text = demo.Name;
            recordingRangeDialog.startNumericUpDown.Maximum = demo.Ticks - 1;
            recordingRangeDialog.endNumericUpDown.Maximum = demo.Ticks - 1;
            recordingRangeDialog.startNumericUpDown.Value = 0;
            recordingRangeDialog.endNumericUpDown.Value = demo.Ticks - 1;

            if (recordingRangeDialog.ShowDialog() == DialogResult.OK)
            {
                RecordingRanges.Add(new RecordingRange(demo.DirectoryName, demo.Name, (int)recordingRangeDialog.startNumericUpDown.Value, (int)recordingRangeDialog.endNumericUpDown.Value, demo.Ticks - 1));
                rangesListView.VirtualListSize = RecordingRanges.Count;
                editRangeButton.Enabled = false;
                deleteRangeButton.Enabled = false;
            }
        }

        private void EditRange()
        {
            if (rangesListView.SelectedIndices.Count != 1)
                return;

            RecordingRange range = RecordingRanges[rangesListView.SelectedIndices[0]];

            recordingRangeDialog.Text = "Edit Recording Range";
            recordingRangeDialog.demoNameTextBox.Text = range.name;
            recordingRangeDialog.startNumericUpDown.Maximum = range.maxTick;
            recordingRangeDialog.endNumericUpDown.Maximum = range.maxTick;
            recordingRangeDialog.startNumericUpDown.Value = range.startTick;
            recordingRangeDialog.endNumericUpDown.Value = range.endTick;

            if (recordingRangeDialog.ShowDialog() == DialogResult.OK)
            {
                range.startTick = (int)recordingRangeDialog.startNumericUpDown.Value;
                range.endTick = (int)recordingRangeDialog.endNumericUpDown.Value;

                range.item.SubItems[1].Text = range.startTick.ToString();
                range.item.SubItems[2].Text = range.endTick.ToString();

                rangesListView.Refresh();
                editRangeButton.Enabled = false;
                deleteRangeButton.Enabled = false;
            }
        }

        private void DeleteRange()
        {
            for (int i = rangesListView.SelectedIndices.Count - 1; i >= 0; i--)
                RecordingRanges.RemoveAt(rangesListView.SelectedIndices[i]);

            rangesListView.VirtualListSize = RecordingRanges.Count;

            rangesListView_SelectedIndexChanged(null, null);
        }

        private void pathBrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                folder.Description = "Please locate a folder where you'd like to load your demos from.";
                folder.SelectedPath = pathTextBox.Text;

                if (folder.ShowDialog() == DialogResult.OK)
                {
                    pathTextBox.Text = folder.SelectedPath;
                    QueryDemos();
                }
            }
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            RefreshDemos();
        }

        private void demosListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lastColumnIndex)
            {
                demoFilesFilter.Reverse();
                demosListView.RedrawItems(0, demosListView.Items.Count - 1, false);
                return;
            }

            switch (e.Column)
            {
                case 0:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).Name.CompareTo(((DemoFile)b.Tag).Name));
                    break;
                case 1:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).ServerName.CompareTo(((DemoFile)b.Tag).ServerName));
                    break;
                case 2:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).ClientName.CompareTo(((DemoFile)b.Tag).ClientName));
                    break;
                case 3:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).MapName.CompareTo(((DemoFile)b.Tag).MapName));
                    break;
                case 4:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).GameDirectory.CompareTo(((DemoFile)b.Tag).GameDirectory));
                    break;
                case 5:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).PlaybackTime.CompareTo(((DemoFile)b.Tag).PlaybackTime));
                    break;
                case 6:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).Ticks.CompareTo(((DemoFile)b.Tag).Ticks));
                    break;
                case 7:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).NetworkProtocol.CompareTo(((DemoFile)b.Tag).DemoProtocol));
                    break;
                case 8:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).NetworkProtocol.CompareTo(((DemoFile)b.Tag).NetworkProtocol));
                    break;
            }

            lastColumnIndex = e.Column;
            demosListView.RedrawItems(0, demosListView.Items.Count - 1, false);
        }

        private void demosListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = demoFilesFilter[e.ItemIndex];
        }

        private void demosListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            addRangeButton.Enabled = demosListView.SelectedIndices.Count == 1;
        }

        private void rangesListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = RecordingRanges[e.ItemIndex].item;
        }

        private void rangesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            editRangeButton.Enabled = rangesListView.SelectedIndices.Count == 1;
            deleteRangeButton.Enabled = rangesListView.SelectedIndices.Count > 0;
        }

        private void addRangeButton_Click(object sender, EventArgs e)
        {
            AddRange();
        }

        private void editRangeButton_Click(object sender, EventArgs e)
        {
            EditRange();
        }

        private void deleteRangeButton_Click(object sender, EventArgs e)
        {
            DeleteRange();
        }

        private void startRecordingButton_Click(object sender, EventArgs e)
        {
            if (RecordingRanges.Count == 0)
            {
                MessageBox.Show("No recording ranges added.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (StartGameManager.Running)
            {
                MessageBox.Show("Game is already running", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void demosContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = demosListView.SelectedIndices.Count != 1;
        }

        private void viewDemosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (demosListView.SelectedIndices.Count != 1)
                return;

            FileSystem.OpenExplorer(((DemoFile)(demoFilesFilter[demosListView.SelectedIndices[0]].Tag)).FullName);
        }

        private void rangesContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = rangesListView.SelectedIndices.Count != 1;
        }

        private void viewRangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rangesListView.SelectedIndices.Count != 1)
                return;

            FileSystem.OpenExplorer(RecordingRanges[rangesListView.SelectedIndices[0]].FullPath);
        }

        private void editRangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditRange();
        }

        private void deleteRangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteRange();
        }
    }
}
