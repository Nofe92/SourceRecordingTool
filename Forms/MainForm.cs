using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
        #region Fields
        public static Profile CurrentProfile;
        public static CultureInfo US = CultureInfo.CreateSpecificCulture("en-US");

        private FileSystemWatcher cfgFileSystemWatcher;
        private FileSystemWatcher customFileSystemWatcher;
        private FileSystemWatcher customDisabledFileSystemWatcher;
        private FileSystemWatcher skyboxFileSystemWatcher;
        private OpenFileDialog openDemoDialog;
        private OpenFileDialog openProfileDialog;
        private SaveFileDialog saveProfileDialog;
        private VDMForm vdmForm;
        private AboutForm aboutForm;
        private bool virtualDubRunning = false;
        private bool disableCustomEvents = false;
        #endregion

        #region Core
        public MainForm()
        {
            InitializeComponent();

            CurrentProfile = Profile.FromFile();

            InitializeGame();
            InitializeConfig();
            InitializeCustom();
            InitializeSkybox();
            Initialize();
        }

        private void Initialize()
        {
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

            vdmForm = new VDMForm();
            aboutForm = new AboutForm();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = "Aron's Source Recording Tool " + Updater.currentVersion.ToString();

            Program.SetWindowTheme(this.TgaListView.Handle, "explorer", null);
            StartGameManager.GameClosed += AfterGame;

            CurrentProfile.UpdateForm(this);

            ResolutionComboBox.TextChanged += new EventHandler(ResolutionComboBox_TextChanged);
            FramerateComboBox.TextChanged += new EventHandler(FramerateComboBox_TextChanged);

            RefreshDatarate();
            RefreshTGASequences(false);

            Updater.CheckForUpdatesASync();
        }

        private void InitializeGame()
        {
            SRTGame CSGO = new SRTGame(730, "Counter-Strike: Global Offensive", "Counter-Strike Global Offensive", "csgo", "csgo.exe", "cs_baggage_skybox_", "cs_tibet", "embassy", "italy", "jungle", "office", "sky_cs15_daylight01_hdr", "sky_cs15_daylight02_hdr", "sky_cs15_daylight03_hdr", "sky_cs15_daylight04_hdr", "sky_day02_05", "sky_day02_05_hdr", "sky_dust", "vertigoblue_hdr", "vertigo", "vertigo_hdr", "vietnam");
            SRTGame CSS = new SRTGame(240, "Counter-Strike: Source", "Counter-Strike Source", "cstrike", "hl2.exe", "assaultup.vmt", "cxup.vmt", "de_cobbleup.vmt", "de_cobble_hdrup.vmt", "de_piranesiup.vmt", "havup.vmt", "italyup.vmt", "jungleup.vmt", "militia_hdrup.vmt", "officeup.vmt", "sky_c17_05up.vmt", "sky_dustup.vmt", "sky_dust_hdrup.vmt", "tidesup.vmt", "trainup.vmt", "train_hdrup.vmt");
            SRTGame DODS = new SRTGame(300, "Day of Defeat: Source", "Day of Defeat Source", "dod", "hl2.exe", "sky_day01_01", "sky_dod_01_hdr", "sky_dod_02_hdr", "sky_dod_03_hdr", "sky_dod_04_hdr", "sky_dod_05_hdr", "sky_dod_06_hdr", "sky_dod_07z_hdr", "sky_dod_07_hdr", "sky_dod_08_hdr", "sky_dod_09_hdr", "sky_dod_10_hdr");
            SRTGame HL2 = new SRTGame(220, "Half-Life 2", "Half-Life 2", "hl2", "hl2.exe", "sky_borealis01", "sky_day01_01", "sky_day01_01_hdr", "sky_day01_04", "sky_day01_04_hdr", "sky_day01_05", "sky_day01_05_hdr", "sky_day01_06", "sky_day01_06_hdr", "sky_day01_07", "sky_day01_07_hdr", "sky_day01_08", "sky_day01_08_hdr", "sky_day01_09", "sky_day01_09_hdr", "sky_day02_01", "sky_day02_01_hdr", "sky_day02_02", "sky_day02_02_hdr", "sky_day02_03", "sky_day02_03_hdr", "sky_day02_04", "sky_day02_04_hdr", "sky_day02_05", "sky_day02_05_hdr", "sky_day02_06", "sky_day02_06_hdr", "sky_day02_07", "sky_day02_07_hdr", "sky_day02_09", "sky_day02_09_hdr", "sky_day02_10", "sky_day02_10_hdr", "sky_day03_01", "sky_day03_01_hdr", "sky_day03_02", "sky_day03_02_hdr", "sky_day03_03", "sky_day03_04", "sky_day03_04_hdr", "sky_day03_05", "sky_day03_05_hdr", "sky_day03_06b", "sky_day03_06b_hdr", "sky_day03_06", "sky_day03_06_hdr", "sky_wasteland02");
            SRTGame HL2DM = new SRTGame(320, "Half-Life 2: Deathmatch", "Half-Life 2 Deathmatch", "hl2mp", "hl2.exe", "sky_borealis01", "sky_day01_01", "sky_day01_01_hdr", "sky_day01_04", "sky_day01_04_hdr", "sky_day01_05", "sky_day01_05_hdr", "sky_day01_06", "sky_day01_06_hdr", "sky_day01_07", "sky_day01_07_hdr", "sky_day01_08", "sky_day01_08_hdr", "sky_day01_09", "sky_day01_09_hdr", "sky_day02_01", "sky_day02_01_hdr", "sky_day02_02", "sky_day02_02_hdr", "sky_day02_03", "sky_day02_03_hdr", "sky_day02_04", "sky_day02_04_hdr", "sky_day02_05", "sky_day02_05_hdr", "sky_day02_06", "sky_day02_06_hdr", "sky_day02_07", "sky_day02_07_hdr", "sky_day02_09", "sky_day02_09_hdr", "sky_day02_10", "sky_day02_10_hdr", "sky_day03_01", "sky_day03_01_hdr", "sky_day03_02", "sky_day03_02_hdr", "sky_day03_03", "sky_day03_04", "sky_day03_04_hdr", "sky_day03_05", "sky_day03_05_hdr", "sky_day03_06b", "sky_day03_06b_hdr", "sky_day03_06", "sky_day03_06_hdr", "sky_wasteland02");
            SRTGame HL2EP1 = new SRTGame(380, "Half-Life 2: Episode One", "Half-Life 2", "episodic", "hl2.exe", "sky_day03_06c", "sky_ep01_00", "sky_ep01_00_hdr", "sky_ep01_01", "sky_ep01_02", "sky_ep01_02_hdr", "sky_ep01_04a", "sky_ep01_04a_hdr", "sky_ep01_04", "sky_ep01_04_hdr", "sky_ep01_citadel_int", "sky_fog");
            SRTGame HL2EP2 = new SRTGame(420, "Half-Life 2: Episode Two", "Half-Life 2", "ep2", "hl2.exe", "sky_ep02_01", "sky_ep02_01_hdr", "sky_ep02_02", "sky_ep02_02_hdr", "sky_ep02_03", "sky_ep02_03_hdr", "sky_ep02_04", "sky_ep02_04_hdr", "sky_ep02_05", "sky_ep02_05_hdr", "sky_ep02_06", "sky_ep02_06_hdr", "sky_ep02_07", "sky_ep02_caves_hdr");
            SRTGame HL2LOSTCOAST = new SRTGame(340, "Half-Life 2: Lost Coast", "Half-Life 2", "lostcoast", "hl2.exe", "sky_lostcoast_hdr");
            SRTGame L4D = new SRTGame(500, "Left 4 Dead", "left 4 dead", "left4dead", "left4dead.exe", "nighturban01_hdr", "sky_day01_09_hdr", "sky_day01_09_ldr", "sky_l4d_rural02_hdr", "sky_l4d_rural02_ldr", "sky_l4d_urban01_hdr", "test_moon_hdr", "urbannightburning_hdr", "urbannightburning_ldr", "urbannightstormhdr", "urbannightstorm_ldr");
            SRTGame L4D2 = new SRTGame(550, "Left 4 Dead 2", "Left 4 Dead 2", "left4dead2", "left4dead2.exe", "sky_l4d_c1_1_hdr", "sky_l4d_c1_2_hdr", "sky_l4d_c2m1_hdr", "sky_l4d_c4m1_hdr", "sky_l4d_c4m4_hdr", "sky_l4d_c5_1_hdr", "sky_l4d_night02_hdr", "sky_l4d_predawn02_hdr", "sky_l4d_c6m1_hdr", "docks_hdr", "highrise_hdr", "river_hdr", "sky_l4d_urban01_hdr", "test_moon_hdr", "urbannightstormhdr", "urbannightstorm_ldr", "sky_coldstream_1_hdr", "sky_coldstream_2_hdr", "sky_day01_09_hdr", "sky_day01_09_ldr", "sky_l4d_rural02_hdr", "sky_l4d_rural02_ldr", "urbannightburning_hdr", "urbannightburning_ldr");
            SRTGame PORTAL = new SRTGame(400, "Portal", "Portal", "portal", "hl2.exe", "bts_custom_cubemap", "sky_escape_01_");
            SRTGame PORTAL2 = new SRTGame(620, "Portal 2", "Portal 2", "portal2", "portal2.exe", "sky_black", "sky_black_nofog", "sky_fog", "sky_l4d_c4m1_hdr", "sky_white");
            SRTGame TF2 = new SRTGame(440, "Team Fortress 2", "Team Fortress 2", "tf", "hl2.exe", "sky_alpinestorm_01", "sky_badlands_01", "sky_dustbowl_01", "sky_dustbowl_01_hdr", "sky_goldrush_01", "sky_granary_01", "sky_granary_01_hdr", "sky_gravel_01", "sky_gravel_01_hdr", "sky_halloween", "sky_halloween_night_01", "sky_halloween_night_01_hdr", "sky_harvest_01", "sky_harvest_01_hdr", "sky_harvest_night_01", "sky_hydro_01", "sky_hydro_01_hdr", "sky_morningsnow_01", "sky_nightfall_01", "sky_night_01", "sky_rainbow_01", "sky_stormfront_01", "sky_tf2_04", "sky_tf2_04_hdr", "sky_trainyard_01", "sky_upward", "sky_upward_hdr", "sky_well_01", "sky_well_01_hdr");

            SRTGame.AllGames = new SRTGame[] { CSGO, CSS, DODS, HL2, HL2DM, HL2EP1, HL2EP2, HL2LOSTCOAST, L4D, L4D2, PORTAL, PORTAL2, TF2 };
            GameComboBox.DataSource = SRTGame.AllGames;
        }

        private void InitializeConfig()
        {
            Directory.CreateDirectory("moviefiles\\cfg");

            foreach (string file in Directory.EnumerateFiles("moviefiles\\cfg", "*.cfg"))
                ConfigComboBox.Items.Add(Path.GetFileName(file));

            cfgFileSystemWatcher = new FileSystemWatcher("moviefiles\\cfg", "*.cfg");
            cfgFileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
            cfgFileSystemWatcher.SynchronizingObject = this;
            cfgFileSystemWatcher.Created += new FileSystemEventHandler(cfgFileSystemWatcher_Created);
            cfgFileSystemWatcher.Deleted += new FileSystemEventHandler(cfgFileSystemWatcher_Deleted);
            cfgFileSystemWatcher.Renamed += new RenamedEventHandler(cfgFileSystemWatcher_Renamed);

            cfgFileSystemWatcher.EnableRaisingEvents = true;
        }

        private void InitializeCustom()
        {
            Directory.CreateDirectory("moviefiles\\custom");
            Directory.CreateDirectory("moviefiles\\custom_disabled");

            foreach (string file in Directory.EnumerateFileSystemEntries("moviefiles\\custom"))
                CustomCheckedListBox.Items.Add(Path.GetFileName(file), true);

            foreach (string file in Directory.EnumerateFileSystemEntries("moviefiles\\custom_disabled"))
                CustomCheckedListBox.Items.Add(Path.GetFileName(file), false);

            customFileSystemWatcher = new FileSystemWatcher("moviefiles\\custom");
            customFileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            customFileSystemWatcher.SynchronizingObject = this;
            customFileSystemWatcher.Created += new FileSystemEventHandler(customFileSystemWatcher_Created);
            customFileSystemWatcher.Deleted += new FileSystemEventHandler(customFileSystemWatcher_Deleted);
            customFileSystemWatcher.Renamed += new RenamedEventHandler(customFileSystemWatcher_Renamed);

            customDisabledFileSystemWatcher = new FileSystemWatcher("moviefiles\\custom_disabled");
            customDisabledFileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            customDisabledFileSystemWatcher.SynchronizingObject = this;
            customDisabledFileSystemWatcher.Created += new FileSystemEventHandler(customFileSystemWatcher_Created);
            customDisabledFileSystemWatcher.Deleted += new FileSystemEventHandler(customFileSystemWatcher_Deleted);
            customDisabledFileSystemWatcher.Renamed += new RenamedEventHandler(customFileSystemWatcher_Renamed);

            customFileSystemWatcher.EnableRaisingEvents = true;
            customDisabledFileSystemWatcher.EnableRaisingEvents = true;

            CustomCheckedListBox.MouseDown += CustomCheckedListBox_MouseDown;
            CustomCheckedListBox.ItemCheck += CustomCheckedListBox_ItemCheck;
        }

        private void InitializeSkybox()
        {
            Directory.CreateDirectory("moviefiles\\skybox");

            foreach (string dir in Directory.EnumerateDirectories("moviefiles\\skybox"))
                SRTSkybox.AddSkyboxByDirectory(dir);

            skyboxFileSystemWatcher = new FileSystemWatcher("moviefiles\\skybox");
            skyboxFileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            skyboxFileSystemWatcher.SynchronizingObject = this;
            skyboxFileSystemWatcher.Created += skyboxFileSystemWatcher_Created;

            skyboxFileSystemWatcher.EnableRaisingEvents = true;
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
        #endregion

        #region MainForm
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
        #endregion

        #region General
        private void gameLabel_Click(object sender, EventArgs e)
        {
            FileSystem.Open(SRTGame.Common);
        }

        private void directXLabel_Click(object sender, EventArgs e)
        {
            FileSystem.Open("dxdiag.exe");
        }
        #endregion

        #region Video
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
        #endregion

        #region Customization
        private void configLabel_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory("moviefiles\\cfg");
        }

        private void customLabel_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory("moviefiles\\custom");
        }

        private void cfgFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            ConfigComboBox.Items.Add(e.Name);
        }

        private void cfgFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            int index = ConfigComboBox.Items.IndexOf(e.OldName);

            if (index == -1)
                ConfigComboBox.Items.Add(e.Name);
            else if (e.Name.EndsWith(".cfg"))
                ConfigComboBox.Items[index] = e.Name;
            else
                ConfigComboBox.Items.RemoveAt(index);

            if (ConfigComboBox.SelectedIndex == -1)
                ConfigComboBox.SelectedIndex = 0;
        }

        private void cfgFileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            ConfigComboBox.Items.Remove(e.Name);

            if (ConfigComboBox.SelectedIndex == -1)
                ConfigComboBox.SelectedIndex = 0;
        }

        private void customFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            disableCustomEvents = true;
            int index = CustomCheckedListBox.Items.IndexOf(e.Name);

            if (index == -1)
                CustomCheckedListBox.Items.Add(e.Name, sender == customFileSystemWatcher);
            else
                CustomCheckedListBox.SetItemChecked(index, sender == customFileSystemWatcher);
            disableCustomEvents = false;
        }

        private void customFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            disableCustomEvents = true;
            int index = CustomCheckedListBox.Items.IndexOf(e.OldName);

            if (index != -1)
                CustomCheckedListBox.Items[index] = e.Name;
            disableCustomEvents = false;
        }

        private void customFileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            disableCustomEvents = true;
            if (!Directory.Exists(String.Concat("moviefiles\\", sender == customFileSystemWatcher ? "custom_disabled\\" : "custom\\", e.Name)) && !File.Exists(String.Concat("moviefiles\\", sender == customFileSystemWatcher ? "custom_disabled\\" : "custom\\", e.Name)))
                CustomCheckedListBox.Items.Remove(e.Name);
            disableCustomEvents = false;
        }

        private void CustomCheckedListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                CustomCheckedListBox.SelectedIndex = CustomCheckedListBox.IndexFromPoint(e.Location);
        }

        private void CustomCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (disableCustomEvents)
                return;

            if ((e.NewValue = e.CurrentValue) == CheckState.Checked)
            {
                if (File.Exists("moviefiles\\custom\\" + CustomCheckedListBox.Items[e.Index]))
                    File.Move("moviefiles\\custom\\" + CustomCheckedListBox.Items[e.Index], "moviefiles\\custom_disabled\\" + CustomCheckedListBox.Items[e.Index]);
                else if (Directory.Exists("moviefiles\\custom\\" + CustomCheckedListBox.Items[e.Index]))
                    Directory.Move("moviefiles\\custom\\" + CustomCheckedListBox.Items[e.Index], "moviefiles\\custom_disabled\\" + CustomCheckedListBox.Items[e.Index]);
            }
            else
            {
                if (File.Exists("moviefiles\\custom_disabled\\" + CustomCheckedListBox.Items[e.Index]))
                    File.Move("moviefiles\\custom_disabled\\" + CustomCheckedListBox.Items[e.Index], "moviefiles\\custom\\" + CustomCheckedListBox.Items[e.Index]);
                else if (Directory.Exists("moviefiles\\custom_disabled\\" + CustomCheckedListBox.Items[e.Index]))
                    Directory.Move("moviefiles\\custom_disabled\\" + CustomCheckedListBox.Items[e.Index], "moviefiles\\custom\\" + CustomCheckedListBox.Items[e.Index]);
            }
        }

        #region CustomContextMenu
        private void customContextMenuStrip_Opened(object sender, EventArgs e)
        {
            bool itemsSelected = CustomCheckedListBox.SelectedIndex != -1;
            viewCustomToolStripMenuItem.Visible = itemsSelected;
            viewContentsCustomToolStripMenuItem.Visible = itemsSelected;
            toolStripSeparator1.Visible = itemsSelected;
        }
        #endregion

        private void viewCustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CustomCheckedListBox.GetItemChecked(CustomCheckedListBox.SelectedIndex))
                FileSystem.Open("explorer.exe", String.Concat("/select,moviefiles\\custom\\", (string)CustomCheckedListBox.SelectedItem));
            else
                FileSystem.Open("explorer.exe", String.Concat("/select,moviefiles\\custom_disabled\\", (string)CustomCheckedListBox.SelectedItem));
        }

        private void viewContentsCustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CustomCheckedListBox.GetItemChecked(CustomCheckedListBox.SelectedIndex))
                FileSystem.Open(String.Concat("moviefiles\\custom\\", (string)CustomCheckedListBox.SelectedItem));
            else
                FileSystem.Open(String.Concat("moviefiles\\custom_disabled\\", (string)CustomCheckedListBox.SelectedItem));
        }

        private void selectAllCustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CustomCheckedListBox.Items.Count; i++)
                CustomCheckedListBox.SetItemChecked(i, true);
        }

        private void selectNoneCustomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CustomCheckedListBox.Items.Count; i++)
                CustomCheckedListBox.SetItemChecked(i, false);
        }
        #endregion

        #region Skybox
        private void skyboxNameLabel_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDirectory("moviefiles\\skybox");
        }

        private void skyboxPictureBox_Click(object sender, EventArgs e)
        {
            SRTSkybox.SkyboxForm.ShowDialog();
            CurrentProfile.UpdateForm(this);
        }

        private void skyboxFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            SRTSkybox.AddSkyboxByDirectory(e.FullPath);
        }
        #endregion

        #region StartGame
        private void startGameButton_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            CurrentProfile.UpdateProfile(this);
            StartGameManager.StartASync();
        }

        private void AfterGame(bool success)
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
                    if (InvokeRequired)
                        Invoke((MethodInvoker)(() => Close()));
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
        #endregion

        #region StartCompiling
        private void startCompilingButton_Click(object sender, EventArgs e)
        {
            if (TgaListView.CheckedItems.Count == 0)
            {
                MessageBox.Show("No TGA-Sequences checked.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartCompiling();
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
            vdub.Exited += AfterCompiling;
            vdub.Start();
        }

        private void AfterCompiling(object sender, EventArgs e)
        {
            ((Process)sender).Dispose();
            virtualDubRunning = false;

            AfterRecording();
        }
        #endregion

        #region Directories
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
        #endregion

        #region TGACompiler

        private void tgaListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                selectAllTgaToolStripMenuItem.PerformClick();
            else if (e.KeyCode == Keys.Enter)
                viewTgaToolStripMenuItem.PerformClick();
            else if (e.KeyCode == Keys.Delete)
                deleteTgaToolStripMenuItem.PerformClick();
            else if (e.KeyCode == Keys.Escape)
                selectNoneTgaToolStripMenuItem.PerformClick();
        }

        #region TGAContextMenu
        private void tgaContextMenuStrip_Opened(object sender, EventArgs e)
        {
            bool itemsSelected = TgaListView.SelectedIndices.Count > 0;

            viewTgaToolStripMenuItem.Visible = itemsSelected;
            deleteTgaToolStripMenuItem.Visible = itemsSelected;
            toolStripSeparator2.Visible = itemsSelected;
        }

        private void viewTgaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in TgaListView.SelectedItems)
                FileSystem.Open("explorer.exe", String.Concat("/select,\"", TgaTextBox.Text, "\\", item.Text, "_0000.tga\""));
        }

        private void deleteTgaToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void selectAllTgaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in TgaListView.Items)
                item.Checked = true;
        }

        private void selectNoneTgaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in TgaListView.Items)
                item.Checked = false;
        }
        #endregion
        #endregion

        #region MenuStrip
        #region Profile
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
                OpenDemo(openDemoDialog.FileName);
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
        #endregion

        #region Edit
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
        #endregion

        #region Options
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
        #endregion

        #region Backup
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
        #endregion

        #region Tools
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
        #endregion

        #region Help
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
        private void viewChangelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Updater.ShowUpdateForm(false);
        }

        private void forumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.Open("https://sourceforge.net/p/sourcerecordingtool/discussion/");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm.ShowDialog();
        }
        #endregion
        #endregion
    }
}
