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
    public class SRTProfile
    {
        public int GameIndex;
        public int DXLevel;
        public int Width;
        public int Height;
        public int Framerate;
        public string[] customItems;
        public string Skyname;
        public string Config;
        public string TgaPath;
        public string VideoPath;
        public string VDubPath;
        public int CodecIndex;
        public int TGAFPSDetectMode;
        public int DefaultCinematicMode;
        public int ScheduledRecordingMode;
        public bool EnableRecording;
        public bool EnableBinds;
        public bool ConfigExecutionOnRecord;
        public bool WireframeWorkaround;
        public bool DemoPlaybackFeatures;
        public bool ThirdPersonFeatures;
        public bool CompileOnGameExit;
        public int AfterRecording;
        public bool CreateBackups;
        public int BackupMode;
        
        public SRTGame Game
        {
            get { return SRTGame.AllGames[GameIndex]; }
        }

        public SRTSkybox Skybox
        {
            get { return SRTSkybox.FindSkyboxByName(Skyname); }
        }

        public static SRTProfile FromFile(string fileName)
        {
            SRTProfile result = new SRTProfile();
            result.Load(fileName);
            return result;
        }

        public void Save(string fileName)
        {
            Directory.CreateDirectory("moviefiles\\profiles");

            using (BinaryWriter bin = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.Unicode))
            {
                bin.Write(Array.IndexOf(SRTGame.AllGames, Game));
                bin.Write(DXLevel);
                bin.Write(Width);
                bin.Write(Height);
                bin.Write(Framerate);
                bin.Write(Config);

                bin.Write((ushort)customItems.Length);
                for (int i = 0; i < customItems.Length; i++)
                    bin.Write(customItems[i]);

                if (Skybox == null)
                    bin.Write("");
                else
                    bin.Write(Skybox.Name);

                bin.Write(TgaPath);
                bin.Write(VideoPath);
                bin.Write(VDubPath);
                bin.Write(CodecIndex);
                bin.Write(TGAFPSDetectMode);
                bin.Write(DefaultCinematicMode);
                bin.Write(ScheduledRecordingMode);
                bin.Write(EnableRecording);
                bin.Write(EnableBinds);
                bin.Write(ConfigExecutionOnRecord);
                bin.Write(WireframeWorkaround);
                bin.Write(DemoPlaybackFeatures);
                bin.Write(ThirdPersonFeatures);
                bin.Write(CompileOnGameExit);
                bin.Write(AfterRecording);
                bin.Write(CreateBackups);
                bin.Write(BackupMode);
            }
        }

        public void Load(string fileName)
        {
            try
            {
                using (BinaryReader bin = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.Unicode))
                {
                    GameIndex = bin.ReadInt32();
                    DXLevel = bin.ReadInt32();
                    Width = bin.ReadInt32();
                    Height = bin.ReadInt32();
                    Framerate = bin.ReadInt32();
                    Config = bin.ReadString();

                    customItems = new string[bin.ReadUInt16()];

                    for (int i = 0; i < customItems.Length; i++)
                        customItems[i] = bin.ReadString();

                    Skyname = bin.ReadString();
                    TgaPath = bin.ReadString();
                    VideoPath = bin.ReadString();
                    VDubPath = bin.ReadString();
                    CodecIndex = bin.ReadInt32();
                    TGAFPSDetectMode = bin.ReadInt32();
                    DefaultCinematicMode = bin.ReadInt32();
                    ScheduledRecordingMode = bin.ReadInt32();
                    EnableRecording = bin.ReadBoolean();
                    EnableBinds = bin.ReadBoolean();
                    ConfigExecutionOnRecord = bin.ReadBoolean();
                    WireframeWorkaround = bin.ReadBoolean();
                    DemoPlaybackFeatures = bin.ReadBoolean();
                    ThirdPersonFeatures = bin.ReadBoolean();
                    CompileOnGameExit = bin.ReadBoolean();
                    AfterRecording = bin.ReadInt32();
                    CreateBackups = bin.ReadBoolean();
                    BackupMode = bin.ReadInt32();
                }
            }
            catch
            {
                LoadDefault(true);
            }
        }

        public void LoadDefault(bool resetDirs)
        {
            GameIndex = 0;
            DXLevel = 98;
            Width = 1920;
            Height = 1080;
            Framerate = 60;
            Config = "tf2-movie.cfg";
            customItems = new string[0];
            Skyname = "";

            if (resetDirs)
            {
                TgaPath = "recordings\\tga";
                VideoPath = "recordings\\video";
                VDubPath = "";
            }

            CodecIndex = 0;
            TGAFPSDetectMode = 3;
            DefaultCinematicMode = 0;
            ScheduledRecordingMode = 0;
            EnableRecording = true;
            EnableBinds = true;
            ConfigExecutionOnRecord = true;
            WireframeWorkaround = true;
            DemoPlaybackFeatures = true;
            ThirdPersonFeatures = true;
            CompileOnGameExit = false;
            AfterRecording = 0;
            CreateBackups = true;
            BackupMode = 1;
        }

        public void UpdateProfile(MainForm mainForm)
        {
            GameIndex = mainForm.GameComboBox.SelectedIndex;

            switch (mainForm.DirectXComboBox.SelectedIndex)
            {
                case 0:
                    DXLevel = 80;
                    break;
                case 1:
                    DXLevel = 81;
                    break;
                case 2:
                    DXLevel = 90;
                    break;
                case 3:
                    DXLevel = 95;
                    break;
                case 4:
                default:
                    DXLevel = 98;
                    break;
            }

            Config = mainForm.ConfigComboBox.SelectedIndex == 0 ? "" : mainForm.ConfigComboBox.Text;

            CheckedListBox.CheckedItemCollection customCheckedItems = mainForm.CustomCheckedListBox.CheckedItems;
            customItems = new string[customCheckedItems.Count];

            for (int i = 0; i < customCheckedItems.Count; i++)
                customItems[i] = (string)customCheckedItems[i];

            TgaPath = mainForm.TgaTextBox.Text;
            VideoPath = mainForm.VideoTextBox.Text;
            CodecIndex = mainForm.CodecComboBox.SelectedIndex;

            EnableRecording = mainForm.EnableRecordingToolStripMenuItem.Checked;
            EnableBinds = mainForm.EnableBindsToolStripMenuItem.Checked;
            ConfigExecutionOnRecord = mainForm.ConfigExecutionOnRecordToolStripMenuItem.Checked;
            WireframeWorkaround = mainForm.WireframeWorkaroundToolStripMenuItem.Checked;
            DemoPlaybackFeatures = mainForm.DemoPlaybackFeaturesToolStripMenuItem.Checked;
            ThirdPersonFeatures = mainForm.ThirdPersonFeaturesToolStripMenuItem.Checked;
            CompileOnGameExit = mainForm.CompileOnGameExitToolStripMenuItem.Checked;
            CreateBackups = mainForm.EnableBackupsToolStripMenuItem.Checked;
        }

        public void UpdateForm(MainForm mainForm)
        {
            try
            {
                InternalUpdateForm(mainForm);
            }
            catch
            {
                LoadDefault(true);
                InternalUpdateForm(mainForm);
            }
        }

        private void InternalUpdateForm(MainForm mainForm)
        {
            mainForm.GameComboBox.SelectedIndex = Array.IndexOf(SRTGame.AllGames, Game);

            switch (DXLevel)
            {
                case 80:
                    mainForm.DirectXComboBox.SelectedIndex = 0;
                    break;
                case 81:
                    mainForm.DirectXComboBox.SelectedIndex = 1;
                    break;
                case 90:
                    mainForm.DirectXComboBox.SelectedIndex = 2;
                    break;
                case 95:
                    mainForm.DirectXComboBox.SelectedIndex = 3;
                    break;
                case 98:
                default:
                    mainForm.DirectXComboBox.SelectedIndex = 4;
                    break;
            }

            mainForm.ResolutionComboBox.Text = Width + "x" + Height;
            mainForm.FramerateComboBox.Text = Framerate.ToString();

            if (Config == "")
                mainForm.ConfigComboBox.SelectedIndex = 0;
            else
                mainForm.ConfigComboBox.SelectedItem = Config;

            CheckedListBox.ObjectCollection customItemsCollection = mainForm.CustomCheckedListBox.Items;
            for (int i = 0; i < customItemsCollection.Count; i++)
            {
                string currentItem = (string)customItemsCollection[i];
                bool found = false;

                for (int j = 0; j < customItems.Length; j++)
                {
                    if (currentItem == customItems[j])
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    mainForm.CustomCheckedListBox.SetItemChecked(i, true);
                else
                    mainForm.CustomCheckedListBox.SetItemChecked(i, false);
            }

            if (Skybox != null)
                mainForm.SkyboxPictureBox.Image = Skybox.PreviewImage;
            else
                mainForm.SkyboxPictureBox.Image = Properties.Resources.defaultskybox;

            mainForm.TgaTextBox.Text = TgaPath;
            mainForm.VideoTextBox.Text = VideoPath;
            mainForm.CodecComboBox.SelectedIndex = CodecIndex;

            switch (TGAFPSDetectMode)
            {
                case 0:
                    mainForm.AbsoluteToolStripMenuItem.PerformClick();
                    break;
                case 1:
                    mainForm.AbsoluteRoundedToolStripMenuItem.PerformClick();
                    break;
                case 2:
                    mainForm.LinearToolStripMenuItem.PerformClick();
                    break;
                case 3:
                default:
                    mainForm.ExponentialToolStripMenuItem.PerformClick();
                    break;
            }

            switch (DefaultCinematicMode)
            {
                case 0:
                default:
                    mainForm.OffToolStripMenuItem.PerformClick();
                    break;
                case 1:
                    mainForm.NormalToolStripMenuItem.PerformClick();
                    break;
                case 2:
                    mainForm.ExtremeToolStripMenuItem.PerformClick();
                    break;
            }

            mainForm.EnableRecordingToolStripMenuItem.Checked = EnableRecording;

            if (!EnableRecording)
                mainForm.EnableRecordingToolStripMenuItem_CheckedChanged(null, null);

            mainForm.EnableBindsToolStripMenuItem.Checked = EnableBinds;

            if (!EnableBinds)
                mainForm.EnableBindsToolStripMenuItem_CheckedChanged(null, null);

            mainForm.ConfigExecutionOnRecordToolStripMenuItem.Checked = ConfigExecutionOnRecord;
            mainForm.WireframeWorkaroundToolStripMenuItem.Checked = WireframeWorkaround;
            mainForm.DemoPlaybackFeaturesToolStripMenuItem.Checked = DemoPlaybackFeatures;
            mainForm.ThirdPersonFeaturesToolStripMenuItem.Checked = ThirdPersonFeatures;
            mainForm.CompileOnGameExitToolStripMenuItem.Checked = CompileOnGameExit;

            switch (AfterRecording)
            {
                case 0:
                default:
                    mainForm.DoNothingToolStripMenuItem.PerformClick();
                    break;
                case 1:
                    mainForm.CloseApplicationToolStripMenuItem.PerformClick();
                    break;
                case 2:
                    mainForm.StandbyToolStripMenuItem.PerformClick();
                    break;
                case 3:
                    mainForm.HibernateToolStripMenuItem.PerformClick();
                    break;
                case 4:
                    mainForm.ShutDownToolStripMenuItem.PerformClick();
                    break;
            }

            mainForm.EnableBackupsToolStripMenuItem.Checked = CreateBackups;

            if (!CreateBackups)
                mainForm.CreateBackupsToolStripMenuItem_CheckedChanged(null, null);

            switch (BackupMode)
            {
                case 0:
                    mainForm.OnceToolStripMenuItem.PerformClick();
                    break;
                case 1:
                default:
                    mainForm.DailyToolStripMenuItem.PerformClick();
                    break;
                case 2:
                    mainForm.EverytimeToolStripMenuItem.PerformClick();
                    break;
            }

            if (mainForm.GameComboBox.SelectedIndex == -1)
                mainForm.GameComboBox.SelectedIndex = 0;

            if (mainForm.DirectXComboBox.SelectedIndex == -1)
                mainForm.DirectXComboBox.SelectedIndex = 4;

            if (mainForm.ConfigComboBox.SelectedIndex == -1)
                mainForm.ConfigComboBox.SelectedIndex = 0;

            if (mainForm.CodecComboBox.SelectedIndex == -1)
                mainForm.CodecComboBox.SelectedIndex = 0;
        }
    }
}
