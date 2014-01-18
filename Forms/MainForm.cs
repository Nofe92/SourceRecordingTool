using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SourceRecordingTool
{
    public partial class MainForm : Form
    {
        public static Profile CurrentProfile;
        internal static System.Globalization.CultureInfo US = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

        private OpenFileDialog openDemoDialog;
        private OpenFileDialog openProfileDialog;
        private SaveFileDialog saveProfileDialog;
        private SkyboxForm skyboxForm;
        private VDMForm vdmForm;
        private AboutForm aboutForm;
        private bool virtualDubRunning = false;

        public MainForm()
        {
            InitializeComponent();
            Initialize();

            CurrentProfile = Profile.FromFile();
            SRTGame.Initialize(this);
            SRTConfig.Initialize(this);
            SRTCustom.Initialize(this);
            SRTSkybox.Initialize();
            CurrentProfile.UpdateForm(this);

            skyboxForm = new SkyboxForm();
            vdmForm = new VDMForm();
            aboutForm = new AboutForm();

            ResolutionComboBox.TextChanged += new EventHandler(ResolutionComboBox_TextChanged);
            FramerateComboBox.TextChanged += new EventHandler(FramerateComboBox_TextChanged);

            RefreshDatarate();
            RefreshTGASequences(false);

            Updater.CheckForUpdatesASync();
        }

        private void Initialize()
        {
            Program.SetWindowTheme(this.TgaListView.Handle, "explorer", null);

            openDemoDialog = new OpenFileDialog();
            openProfileDialog = new OpenFileDialog();
            saveProfileDialog = new SaveFileDialog();

            openDemoDialog.Filter = "DEM File|*.dem";
            openDemoDialog.InitialDirectory = SRTGame.Common;
            openDemoDialog.Title = "Open Demo";

            openProfileDialog.Filter = saveProfileDialog.Filter = "Source Recording Tool Profile|*.dat";
            openProfileDialog.InitialDirectory = saveProfileDialog.InitialDirectory = Path.GetFullPath("moviefiles\\profiles");
            openProfileDialog.Title = "Open Profile";
            saveProfileDialog.Title = "Save Profile";

            this.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            this.Text = "Aron's Source Recording Tool " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            StartGameManager.GameClosed += StartGameManager_GameClosed;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            CurrentProfile.UpdateProfile(this);
            CurrentProfile.Save();
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
                drgevent.Effect = DragDropEffects.Move;
            else
                drgevent.Effect = DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            string[] paths = (string[])drgevent.Data.GetData(DataFormats.FileDrop);

            OpenDemo(paths[0]);
        }

        private void RefreshDatarate()
        {
            resolutionLabel.ForeColor = CurrentProfile.Width < 0 ? Color.DarkRed : Color.Navy;
            ResolutionComboBox.BackColor = CurrentProfile.Width < 0 ? Color.LightCoral : Color.FromKnownColor(KnownColor.Window);
            framerateLabel.ForeColor = CurrentProfile.Framerate < 0 ? Color.DarkRed : Color.Navy;
            FramerateComboBox.BackColor = CurrentProfile.Framerate < 0 ? Color.LightCoral : Color.FromKnownColor(KnownColor.Window);

            if (CurrentProfile.Width < 0 || CurrentProfile.Framerate < 0)
            {
                startGameButton.Enabled = false;
                datarateValueLabel.Text = "";
            }
            else
            {
                startGameButton.Enabled = true;

                long size = CurrentProfile.Width * CurrentProfile.Height * 3L * CurrentProfile.Framerate + 44100L * 2L * 2L;
                string pathRoot;

                if (TgaTextBox.Text == "" || !Directory.Exists(pathRoot = Path.GetPathRoot(Path.GetFullPath(TgaTextBox.Text))))
                    datarateValueLabel.Text = String.Format("{0:F2} MB/sec", size / (1024d * 1024d));
                else
                    datarateValueLabel.Text = String.Format("{0:F2} MB/sec ({1} minutes left)", size / (1024d * 1024d), new DriveInfo(pathRoot).AvailableFreeSpace / size / 60);
            }

            if (Directory.Exists("backup"))
                clearBackupCacheToolStripMenuItem.Text = String.Concat("Clear Backup cache (", (new DirectoryInfo("backup").EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(f => f.Length) - 1) / 1024 / 1024 + 1, " MB)...");
            else
                clearBackupCacheToolStripMenuItem.Text = "Clear Backup cache...";
        }

        private void RefreshTGASequences(bool clear)
        {
            if (clear)
                TgaListView.Items.Clear();

            if (!Directory.Exists(TgaTextBox.Text))
                return;

            string name, fileName;

            for (int i = 1; i <= 26; i++)
            {
                name = ((char)(0x60 + i)).ToString() + i.ToString();
                ListViewItem item = TgaListView.FindItemWithText(name);

                if (item != null)
                    continue;

                if (File.Exists((fileName = String.Concat(TgaTextBox.Text, "\\", name)) + "_0000.tga") && File.Exists(fileName + "_.wav"))
                {
                    string[] files = Directory.GetFiles(TgaTextBox.Text, name + "_*.tga");
                    DateTime start = File.GetCreationTime(files[0]);
                    DateTime end = File.GetCreationTime(files[files.Length - 1]);
                    TimeSpan span = end - start;

                    FileStream wav = new FileStream(fileName + "_.wav", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    wav.Position = 40;

                    byte[] wavSizeBuf = new byte[4];
                    wav.Read(wavSizeBuf, 0, 4);
                    wav.Close();

                    item = new ListViewItem();
                    item.Text = name;
                    item.SubItems.Add(files.Length.ToString());

                    switch (CurrentProfile.TGAFPSDetectMode)
                    {
                        case 0:
                            item.SubItems.Add(Math.Round((double)files.Length / (double)(BitConverter.ToInt32(wavSizeBuf, 0) / 44100d / 4d), 3, MidpointRounding.AwayFromZero).ToString(US));
                            break;
                        case 1:
                            item.SubItems.Add(Math.Round((double)files.Length / (double)(BitConverter.ToInt32(wavSizeBuf, 0) / 44100d / 4d), 0, MidpointRounding.AwayFromZero).ToString(US));
                            break;
                        case 2:
                            item.SubItems.Add((30d * Math.Round((double)files.Length / (double)(BitConverter.ToInt32(wavSizeBuf, 0) / 44100d / 4d) / 30d, MidpointRounding.AwayFromZero)).ToString(US));
                            break;
                        case 3:
                            item.SubItems.Add((30d * Math.Pow(2d, Math.Round(Math.Log((double)files.Length / (double)(BitConverter.ToInt32(wavSizeBuf, 0) / 44100d / 4d) / 30d, 2d), MidpointRounding.AwayFromZero))).ToString(US));
                            break;
                    }

                    item.SubItems.Add(start.ToString());
                    item.SubItems.Add(span.ToString("hh\\:mm\\:ss"));
                    item.SubItems.Add((files.Length / span.TotalSeconds).ToString("F2", US));
                    item.Checked = true;

                    TgaListView.Items.Add(item);
                }
            }
        }

        private void OpenDemo(string path)
        {
            if (!path.EndsWith(".dem"))
            {
                MessageBox.Show("Invalid demo file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] gameBuffer = new byte[260];

            FileStream demo = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            demo.Position = 8 + 4 + 4 + 3 * 260;
            demo.Read(gameBuffer, 0, 260);

            string shortName = Encoding.ASCII.GetString(gameBuffer).Trim('\0');

            for (int i = 0; i < SRTGame.AllGames.Length; i++)
            {
                if (SRTGame.AllGames[i].ShortName == shortName)
                {
                    CurrentProfile.UpdateProfile(this);
                    CurrentProfile.GameIndex = i;
                    CurrentProfile.UpdateForm(this);

                    StartGameManager.Demo = path;
                    StartGameManager.StartASync();
                    return;
                }
            }

            MessageBox.Show("Game not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void StartCompiling()
        {
            if (TgaListView.CheckedItems.Count == 0)
                return;

            if (!File.Exists(CurrentProfile.VDubPath))
            {
                using (OpenFileDialog open = new OpenFileDialog())
                {
                    open.Title = "In order to compile, you need to locate VirtualDub";
                    open.Filter = "VirtualDub|VirtualDub.exe;Veedub64.exe;vdub.exe;vdub64.exe";
                    open.FileName = CurrentProfile.VDubPath;

                    if (open.ShowDialog() == DialogResult.OK)
                        CurrentProfile.VDubPath = open.FileName;
                    else
                        return;
                }
            }

            if (!Directory.Exists(VideoTextBox.Text))
                Directory.CreateDirectory(VideoTextBox.Text);

            if (virtualDubRunning)
            {
                MessageBox.Show("VirtualDub Compiler is already running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            virtualDubRunning = true;

            string jobsPath = Path.GetDirectoryName(CurrentProfile.VDubPath) + "\\SourceRecordingTool_jobs.txt";
            StreamWriter jobsStream = new StreamWriter(jobsPath);

            foreach (ListViewItem item in TgaListView.CheckedItems)
            {
                string firstFrame = String.Concat(TgaTextBox.Text, "\\", item.Text, "_0000.tga");

                jobsStream.WriteLine("VirtualDub.Open(\"{0}\",\"\",0);", firstFrame.Replace("\\", "\\\\"));
                jobsStream.WriteLine("VirtualDub.audio.SetSource(\"{0}\", \"\");", (String.Concat(TgaTextBox.Text, "\\", item.Text, "_.WAV")).Replace("\\", "\\\\"));
                jobsStream.WriteLine("VirtualDub.audio.SetMode(0);");
                jobsStream.WriteLine("VirtualDub.audio.SetInterleave(1,500,1,0,0);");
                jobsStream.WriteLine("VirtualDub.audio.SetClipMode(1,1);");
                jobsStream.WriteLine("VirtualDub.audio.SetEditMode(1);");
                jobsStream.WriteLine("VirtualDub.audio.SetConversion(0,0,0,0,0);");
                jobsStream.WriteLine("VirtualDub.audio.SetVolume();");
                jobsStream.WriteLine("VirtualDub.audio.SetCompression();");
                jobsStream.WriteLine("VirtualDub.audio.EnableFilterGraph(0);");
                jobsStream.WriteLine("VirtualDub.video.SetInputFormat(0);");
                jobsStream.WriteLine("VirtualDub.video.SetOutputFormat(7);");
                jobsStream.WriteLine("VirtualDub.video.SetMode(3);");
                jobsStream.WriteLine("VirtualDub.video.SetSmartRendering(0);");
                jobsStream.WriteLine("VirtualDub.video.SetPreserveEmptyFrames(0);");
                jobsStream.WriteLine("VirtualDub.video.SetFrameRate2({0},1,1);", item.SubItems[2].Text);
                jobsStream.WriteLine("VirtualDub.video.SetIVTC(0, 0, 0, 0);");

                switch (CodecComboBox.SelectedIndex)
                {
                    case 0:
                        jobsStream.WriteLine("VirtualDub.video.SetCompression();");
                        break;
                    case 1:
                        jobsStream.WriteLine("VirtualDub.video.SetCompression(0x7367616c,0,10000,0);");
                        jobsStream.WriteLine("VirtualDub.video.SetCompData(1,\"AA==\");");
                        break;
                    default:
                        jobsStream.WriteLine("VirtualDub.video.SetCompression();");
                        break;
                }

                jobsStream.WriteLine("VirtualDub.video.filters.Clear();");
                jobsStream.WriteLine("VirtualDub.audio.filters.Clear();");
                jobsStream.WriteLine("VirtualDub.subset.Clear();");
                jobsStream.WriteLine("VirtualDub.subset.AddRange(0,{0});", item.SubItems[1].Text);
                jobsStream.WriteLine("VirtualDub.video.SetRange();");
                jobsStream.WriteLine("VirtualDub.project.ClearTextInfo();");
                jobsStream.WriteLine("VirtualDub.SaveAVI(\"{0}\");", (String.Concat(VideoTextBox.Text, "\\", File.GetCreationTime(firstFrame).ToString("yyyy-MM-dd HH-mm-ss"), " ", item.Text, ".avi")).Replace("\\", "\\\\"));
                jobsStream.WriteLine("VirtualDub.Close();");
                jobsStream.WriteLine();
            }

            jobsStream.Close();

            Process vdub = new Process();
            vdub.StartInfo = new ProcessStartInfo(CurrentProfile.VDubPath, String.Concat("/s \"", jobsPath, "\" /x"));
            vdub.EnableRaisingEvents = true;
            vdub.Exited += vdub_Exited;
            vdub.Start();
        }

        private void StartGameManager_GameClosed(bool success)
        {
            if (!success)
                return;

            if (InvokeRequired)
                Invoke((MethodInvoker)(() =>
                {
                    RefreshDatarate();
                    RefreshTGASequences(false);

                    if (CurrentProfile.CompileOnGameExit)
                        StartCompiling();
                    else
                        AfterRecording();
                }));
        }

        private void AfterRecording()
        {
            switch (CurrentProfile.AfterRecording)
            {
                case 0:
                default:
                    break;
                case 1:
                    Close();
                    break;
                case 2:
                    Application.SetSuspendState(PowerState.Suspend, false, false);
                    break;
                case 3:
                    Application.SetSuspendState(PowerState.Hibernate, false, false);
                    break;
                case 4:
                    Program.ExitWindowsEx(1, 0);
                    break;
            }
        }

        private void vdub_Exited(object sender, EventArgs e)
        {
            ((Process)sender).Dispose();
            virtualDubRunning = false;

            if (InvokeRequired)
                Invoke((MethodInvoker)(() => AfterRecording()));
        }

        private void gameLabel_Click(object sender, EventArgs e)
        {
            FileSystem.Open(SRTGame.Common);
        }

        private void directXLabel_Click(object sender, EventArgs e)
        {
            FileSystem.Open("dxdiag.exe");
        }

        private void ResolutionComboBox_TextChanged(object sender, EventArgs e)
        {
            string[] dimensions = ResolutionComboBox.Text.Split('x');

            if (dimensions.Length != 2 || !int.TryParse(dimensions[0].Trim(), out CurrentProfile.Width) || !int.TryParse(dimensions[1].Trim(), out CurrentProfile.Height) && CurrentProfile.Width > 0 && CurrentProfile.Height > 0)
                CurrentProfile.Width = -1;

            RefreshDatarate();
        }

        private void FramerateComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(FramerateComboBox.Text, out CurrentProfile.Framerate))
                CurrentProfile.Framerate = -1;

            RefreshDatarate();
        }

        private void configLabel_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory("moviefiles\\cfg");
        }

        private void customLabel_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory("moviefiles\\custom");
        }

        private void customListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                if (File.Exists("moviefiles\\custom_disabled\\" + e.Item.Text))
                    File.Move("moviefiles\\custom_disabled\\" + e.Item.Text, "moviefiles\\custom\\" + e.Item.Text);
                else if (Directory.Exists("moviefiles\\custom_disabled\\" + e.Item.Text))
                    Directory.Move("moviefiles\\custom_disabled\\" + e.Item.Text, "moviefiles\\custom\\" + e.Item.Text);
            }
            else
            {
                if (File.Exists("moviefiles\\custom\\" + e.Item.Text))
                    File.Move("moviefiles\\custom\\" + e.Item.Text, "moviefiles\\custom_disabled\\" + e.Item.Text);
                else if (Directory.Exists("moviefiles\\custom\\" + e.Item.Text))
                    Directory.Move("moviefiles\\custom\\" + e.Item.Text, "moviefiles\\custom_disabled\\" + e.Item.Text);
            }
        }

        private void skyboxNameLabel_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory("moviefiles\\skybox");
        }

        private void skyboxPictureBox_Click(object sender, EventArgs e)
        {
            skyboxForm.ShowDialog();
            CurrentProfile.UpdateForm(this);
        }

        private void startGameButton_Click(object sender, EventArgs e)
        {
            CurrentProfile.UpdateProfile(this);
            StartGameManager.StartASync();
        }

        private void tgaLabel_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory(TgaTextBox.Text);
        }

        private void aviLabel_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory(VideoTextBox.Text);
        }

        private void TgaTextBox_Leave(object sender, EventArgs e)
        {
            RefreshDatarate();
            RefreshTGASequences(false);
        }

        private void tgaBrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                folder.Description = "Please locate a folder where you'd like your TGA-Sequences to be saved.";
                folder.SelectedPath = TgaTextBox.Text;

                if (folder.ShowDialog() == DialogResult.OK)
                    TgaTextBox.Text = folder.SelectedPath;
            }

            RefreshTGASequences(false);
        }

        private void aviBrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folder = new FolderBrowserDialog())
            {
                folder.Description = "Please locate a folder where you'd like your AVI-Videos & MP4-Videos to be saved.";
                folder.SelectedPath = VideoTextBox.Text;

                if (folder.ShowDialog() == DialogResult.OK)
                    VideoTextBox.Text = folder.SelectedPath;
            }
        }

        private void tgaListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                selectAllToolStripMenuItem.PerformClick();
            else if (e.KeyCode == Keys.Enter)
                viewToolStripMenuItem.PerformClick();
            else if (e.KeyCode == Keys.Delete)
                deleteToolStripMenuItem.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                selectNoneToolStripMenuItem.PerformClick();
        }

        private void startCompilingButton_Click(object sender, EventArgs e)
        {
            if (TgaListView.CheckedItems.Count == 0)
            {
                MessageBox.Show("No TGA-Sequences checked.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartCompiling();
        }

        private void tgaContextMenuStrip_Opened(object sender, EventArgs e)
        {
            bool itemsSelected = TgaListView.SelectedIndices.Count > 0;

            viewToolStripMenuItem.Visible = itemsSelected;
            deleteToolStripMenuItem.Visible = itemsSelected;
            toolStripSeparator1.Visible = itemsSelected;
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in TgaListView.SelectedItems)
                FileSystem.Open("explorer.exe", String.Concat("/select,\"", TgaTextBox.Text, "\\", item.Text, "_0000.tga\""));
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TgaListView.SelectedItems.Count == 0)
                return;

            StringBuilder items = new StringBuilder();

            for (int i = 0; i < TgaListView.SelectedItems.Count - 1; i++)
            {
                items.Append(TgaListView.SelectedItems[i].Text);
                items.Append(", ");
            }

            items.Append(TgaListView.SelectedItems[TgaListView.SelectedItems.Count - 1].Text);

            if (MessageBox.Show("Are you sure to permanently delete the following TGA-Sequences: " + items.ToString(), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (ListViewItem item in TgaListView.SelectedItems)
                {
                    foreach (string file in Directory.EnumerateFiles(TgaTextBox.Text, item.Text + "_*.tga"))
                        File.Delete(file);

                    File.Delete(String.Concat(TgaTextBox.Text, "\\", item.Text, "_.wav"));

                    item.Remove();
                }
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in TgaListView.Items)
                item.Checked = true;
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in TgaListView.Items)
                item.Checked = false;
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProfile.Load();
            CurrentProfile.UpdateForm(this);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProfile.UpdateProfile(this);
            CurrentProfile.Save();
        }

        private void openDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openDemoDialog.ShowDialog() == DialogResult.OK)
            {
                OpenDemo(openDemoDialog.FileName);
            }
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openProfileDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentProfile.Load(openProfileDialog.FileName);
                CurrentProfile.UpdateForm(this);
            }
        }

        private void saveAsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveProfileDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentProfile.UpdateProfile(this);
                CurrentProfile.Save(saveProfileDialog.FileName);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProfile.LoadDefault(false);
            CurrentProfile.UpdateForm(this);
        }

        private void resetAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to reset everything?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            CurrentProfile.LoadDefault(true);
            CurrentProfile.UpdateForm(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openLongNamePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory(SRTGame.AllGames[GameComboBox.SelectedIndex].LongNamePath);
        }

        private void openShortNamePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory(SRTGame.AllGames[GameComboBox.SelectedIndex].ShortNamePath);
        }

        private void installGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.Open("steam://install/" + SRTGame.AllGames[GameComboBox.SelectedIndex].AppID.ToString());
        }

        private void validateGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.Open("steam://validate/" + SRTGame.AllGames[GameComboBox.SelectedIndex].AppID.ToString());
        }

        private void deleteTGASequencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to permanently delete all TGA-Sequences?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (string file in Directory.EnumerateFiles(TgaTextBox.Text, "*.tga"))
                    File.Delete(file);

                foreach (string file in Directory.EnumerateFiles(TgaTextBox.Text, "*.wav"))
                    File.Delete(file);
            }
        }

        private void deleteAVIVideosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to permanently delete all AVI-Videos?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (string file in Directory.EnumerateFiles(VideoTextBox.Text, "*.avi"))
                    File.Delete(file);
            }
        }

        private void deleteMP4VideosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to permanently delete all MP4-Videos?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (string file in Directory.EnumerateFiles(VideoTextBox.Text, "*.mp4"))
                    File.Delete(file);
            }
        }

        private void tgaFpsDetectionMethod_Changed(object sender, EventArgs e)
        {
            for (int i = 0; i < tgaFpsDetectionMethodToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (tgaFpsDetectionMethodToolStripMenuItem.DropDownItems[i] != sender)
                    ((ToolStripMenuItem)tgaFpsDetectionMethodToolStripMenuItem.DropDownItems[i]).Checked = false;
                else
                {
                    ((ToolStripMenuItem)tgaFpsDetectionMethodToolStripMenuItem.DropDownItems[i]).Checked = true;
                    CurrentProfile.TGAFPSDetectMode = i;
                    RefreshTGASequences(true);
                }
            }
        }

        private void cinematicMode_Changed(object sender, EventArgs e)
        {
            for (int i = 0; i < defaultCinematicModeToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (defaultCinematicModeToolStripMenuItem.DropDownItems[i] != sender)
                    ((ToolStripMenuItem)defaultCinematicModeToolStripMenuItem.DropDownItems[i]).Checked = false;
                else
                {
                    ((ToolStripMenuItem)defaultCinematicModeToolStripMenuItem.DropDownItems[i]).Checked = true;
                    CurrentProfile.DefaultCinematicMode = i;
                }
            }
        }

        public void EnableRecordingToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = EnableRecordingToolStripMenuItem.Checked;
            
            ConfigExecutionOnRecordToolStripMenuItem.Enabled = enabled;
        }

        public void EnableBindsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = EnableBindsToolStripMenuItem.Checked;

            WireframeWorkaroundToolStripMenuItem.Enabled = enabled;
            DemoPlaybackFeaturesToolStripMenuItem.Enabled = enabled;
            ThirdPersonFeaturesToolStripMenuItem.Enabled = enabled;
        }

        private void afterRecording_Changed(object sender, EventArgs e)
        {
            for (int i = 0; i < afterRecordingToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (afterRecordingToolStripMenuItem.DropDownItems[i] != sender)
                    ((ToolStripMenuItem)afterRecordingToolStripMenuItem.DropDownItems[i]).Checked = false;
                else
                {
                    ((ToolStripMenuItem)afterRecordingToolStripMenuItem.DropDownItems[i]).Checked = true;
                    CurrentProfile.AfterRecording = i;
                }
            }
        }

        public void CreateBackupsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            backupModeToolStripMenuItem.Enabled = EnableBackupsToolStripMenuItem.Checked;
        }

        private void backupMode_Changed(object sender, EventArgs e)
        {
            for (int i = 0; i < backupModeToolStripMenuItem.DropDownItems.Count; i++)
            {
                if (backupModeToolStripMenuItem.DropDownItems[i] != sender)
                    ((ToolStripMenuItem)backupModeToolStripMenuItem.DropDownItems[i]).Checked = false;
                else
                {
                    ((ToolStripMenuItem)backupModeToolStripMenuItem.DropDownItems[i]).Checked = true;
                    CurrentProfile.BackupMode = i;
                }
            }
        }

        private void viewBackupFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory("backup");
        }

        private void clearBackupCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to permanently delete all backups?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                FileSystem.DeleteDirectory("backup");
                RefreshDatarate();
            }
        }

        private void vdmCreatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProfile.UpdateProfile(this);
            vdmForm.Initialize();

            if (vdmForm.ShowDialog() == DialogResult.OK)
            {
                MainForm.CurrentProfile.ScheduledRecordingMode = vdmForm.modeComboBox.SelectedIndex;

                StartGameManager.RecordingRanges.Clear();
                StartGameManager.RecordingRanges.AddRange(vdmForm.RecordingRanges);

                StartGameManager.StartASync();
            }
        }

        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.Open("http://www.youtube.com/hl2mukkel");
        }

        private void keyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.Open("moviefiles\\help\\Keyboard.png");
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Updater.CheckForUpdates() == Updater.UpdateState.Latest)
                MessageBox.Show("Your version is up-to-date", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void forumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.Open("https://sourceforge.net/p/sourcerecordingtool/discussion/");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm.ShowDialog();
        }
    }
}
