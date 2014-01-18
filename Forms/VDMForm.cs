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
                        demo.name = Path.GetFileName(file);
                        demo.path = pathTextBox.Text;
                        demo.demoProtocol = demoReader.ReadInt32();
                        demo.networkProtocol = demoReader.ReadInt32();
                        demo.serverName = new string(demoReader.ReadChars(260)).TrimEnd('\0');
                        demo.clientName = new string(demoReader.ReadChars(260)).TrimEnd('\0');
                        demo.mapName = new string(demoReader.ReadChars(260)).TrimEnd('\0');
                        demo.gameDirectory = new string(demoReader.ReadChars(260)).TrimEnd('\0');
                        demo.playbackTime = demoReader.ReadSingle();
                        demo.ticks = demoReader.ReadInt32();

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
                    demo.name,
                    demo.serverName,
                    demo.clientName,
                    demo.mapName,
                    demo.gameDirectory,
                    demo.playbackTime.ToString() + " s",
                    demo.ticks.ToString(),
                    demo.demoProtocol.ToString(),
                    demo.networkProtocol.ToString(),
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
                    demoFiles[i].name.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].serverName.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].clientName.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].mapName.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].gameDirectory.ToLowerInvariant().Contains(match) ||
                    demoFiles[i].playbackTime.ToString().Contains(match) ||
                    demoFiles[i].ticks.ToString().Contains(match) ||
                    demoFiles[i].demoProtocol.ToString().Contains(match) ||
                    demoFiles[i].networkProtocol.ToString().Contains(match)
                    )
                    demoFilesFilter.Add(listViewItemCache[i]);

            demosListView.VirtualListSize = demoFilesFilter.Count;
            addRangeButton.Enabled = false;
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
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).name.CompareTo(((DemoFile)b.Tag).name));
                    break;
                case 1:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).serverName.CompareTo(((DemoFile)b.Tag).serverName));
                    break;
                case 2:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).clientName.CompareTo(((DemoFile)b.Tag).clientName));
                    break;
                case 3:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).mapName.CompareTo(((DemoFile)b.Tag).mapName));
                    break;
                case 4:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).gameDirectory.CompareTo(((DemoFile)b.Tag).gameDirectory));
                    break;
                case 5:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).playbackTime.CompareTo(((DemoFile)b.Tag).playbackTime));
                    break;
                case 6:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).ticks.CompareTo(((DemoFile)b.Tag).ticks));
                    break;
                case 7:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).networkProtocol.CompareTo(((DemoFile)b.Tag).demoProtocol));
                    break;
                case 8:
                    demoFilesFilter.Sort((a, b) => ((DemoFile)a.Tag).networkProtocol.CompareTo(((DemoFile)b.Tag).networkProtocol));
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
            DemoFile demo = (DemoFile)demoFilesFilter[demosListView.SelectedIndices[0]].Tag;

            recordingRangeDialog.Text = "Add Recording Range";
            recordingRangeDialog.demoNameTextBox.Text = demo.name;
            recordingRangeDialog.startNumericUpDown.Maximum = demo.ticks - 1;
            recordingRangeDialog.endNumericUpDown.Maximum = demo.ticks - 1;
            recordingRangeDialog.startNumericUpDown.Value = 0;
            recordingRangeDialog.endNumericUpDown.Value = demo.ticks - 1;

            if (recordingRangeDialog.ShowDialog() == DialogResult.OK)
            {
                RecordingRanges.Add(new RecordingRange(demo.path, demo.name, (int)recordingRangeDialog.startNumericUpDown.Value, (int)recordingRangeDialog.endNumericUpDown.Value, demo.ticks - 1));
                rangesListView.VirtualListSize = RecordingRanges.Count;
                editRangeButton.Enabled = false;
                deleteRangeButton.Enabled = false;
            }
        }

        private void editRangeButton_Click(object sender, EventArgs e)
        {
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

        private void deleteRangeButton_Click(object sender, EventArgs e)
        {
            for (int i = rangesListView.SelectedIndices.Count - 1; i >= 0; i--)
                RecordingRanges.RemoveAt(rangesListView.SelectedIndices[i]);

            rangesListView.VirtualListSize = RecordingRanges.Count;

            rangesListView_SelectedIndexChanged(null, null);
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
    }
}
