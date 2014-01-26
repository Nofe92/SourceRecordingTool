using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SourceRecordingTool
{
    public static class SRTStartGameManager
    {
        public static List<RecordingRange> RecordingRanges = new List<RecordingRange>();
        private static List<RecordingRange> localRecordingRange = new List<RecordingRange>();

        public static bool Running = false;
        private static List<string> vdmFileList = new List<string>();
        private static SRTProfile profile;
        private static SRTGame game;
        private static string demo;
        private static string addons;
        private static string cfg;
        private static string custom;
        private static RegistryKey registryKey;
        private static string[] regValueNames;
        private static object[] regValues;
        
        public delegate void GameClosedEventHandler(bool success);

        public static event GameClosedEventHandler GameClosed;

        public static void StartASync()
        {
            if (Running)
            {
                Dialogs.Warning("Game is already running");
                return;
            }

            Running = true;
            Thread startThread = new Thread(Start);
            startThread.Start();
        }

        public static void StartASync(string demoFile)
        {
            if (!demoFile.EndsWith(".dem"))
            {
                Dialogs.Error("Invalid demo file.");
                return;
            }

            byte[] gameBuffer = new byte[260];

            using (FileStream demoFileStream = new FileStream(demoFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                demoFileStream.Position = 8 + 4 + 4 + 3 * 260;
                demoFileStream.Read(gameBuffer, 0, 260);
            }

            string shortName = Encoding.ASCII.GetString(gameBuffer).Trim('\0');

            for (int i = 0; i < SRTGame.AllGames.Length; i++)
            {
                if (SRTGame.AllGames[i].ShortName == shortName)
                {
                    MainForm.CurrentProfile.GameIndex = i;
                    demo = demoFile;
                    StartASync();

                    return;
                }
            }

            Dialogs.Error("Game not found.");
        }

        private static void Start()
        {
            try
            {
                Initialize();
                EnsureState();

                if (profile.CreateBackups)
                    CreateBackup();

                MoveFolders();
                MoveRegistry();

                try
                {
                    CreateAddons();
                    CreateConfig();
                    CreateCustom();
                    CreateSkybox();
                    CreateVDM();

                    StartGame();
                }
                catch (Exception ex)
                {
                    Running = false;

                    if (GameClosed != null)
                        GameClosed(false);

                    Dialogs.Error(ex.Message);
                }

                MoveH264Videos();
                MoveRegistryBack();
                MoveFoldersBack();
                Cleanup();
            }
            catch (Exception ex)
            {
                Running = false;

                if (GameClosed != null)
                    GameClosed(false);

                Dialogs.Error(ex.Message);
            }

            Running = false;

            if (GameClosed != null)
                GameClosed(true);
        }

        private static void Initialize()
        {
            profile = MainForm.CurrentProfile;
            game = profile.Game;

            addons = game.ShortNamePath + "\\addons";
            cfg = game.ShortNamePath + "\\cfg";
            custom = game.ShortNamePath + "\\custom";
        }

        private static void EnsureState()
        {
            if (profile.EnableRecording)
            {
                if (profile.TgaPath == "" || profile.VideoPath == "")
                    throw new Exception("TGA Directory and Video Directory must be set before launching the game when recording is enabled.");

                Directory.CreateDirectory(profile.TgaPath);
                Directory.CreateDirectory(profile.VideoPath);
            }

            Process[] hl2 = Process.GetProcessesByName(game.Executable.Substring(0, game.Executable.Length - 4));

            foreach (Process process in hl2)
                process.Dispose();

            if (hl2.Length != 0)
                throw new Exception("Only one instance of the game can be running at one time.");

            if (!File.Exists(game.HL2FileName))
                throw new Exception("Unable to find executable.");
            
            if (DirectoryEx.Exists(addons + "_play") || DirectoryEx.Exists(cfg + "_play") || DirectoryEx.Exists(custom + "_play"))
                throw new Exception(String.Format("Corrupt State:\r\n\r\nAn _play folder still exists in:\r\n{0}\r\n\r\nplease delete or move it before launching the game", game.ShortNamePath));
        }

        private static void CreateBackup()
        {
            string addons_backup, cfg_backup, custom_backup;
            DateTime now = DateTime.Now;

            switch (profile.BackupMode)
            {
                case 0:
                    addons_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy") +  "\\addons";
                    cfg_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy") +  "\\cfg";
                    custom_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy") +  "\\custom";
                    break;
                case 1:
                default:
                    addons_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy-MM-dd") +  "\\addons";
                    cfg_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy-MM-dd") +  "\\cfg";
                    custom_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy-MM-dd") +  "\\custom";
                    break;
                case 2:
                    addons_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy-MM-dd HH-mm-ss") +  "\\addons";
                    cfg_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy-MM-dd HH-mm-ss") +  "\\cfg";
                    custom_backup = "backup\\" +  game.ShortName +  "\\" +  now.ToString("yyyy-MM-dd HH-mm-ss") +  "\\custom";
                    break;
            }

            if (DirectoryEx.Exists(addons) && !DirectoryEx.Exists(addons_backup))
                DirectoryEx.Copy(addons, addons_backup);

            if (DirectoryEx.Exists(cfg) && !DirectoryEx.Exists(cfg_backup))
                DirectoryEx.Copy(cfg, cfg_backup);

            if (DirectoryEx.Exists(custom) && !DirectoryEx.Exists(custom_backup))
                DirectoryEx.Copy(custom, custom_backup);
        }

        private static void MoveFolders()
        {
            Win32.MoveFile(addons, addons + "_play");
            Win32.MoveFile(cfg, cfg + "_play");
            Win32.MoveFile(custom, custom + "_play");

            DirectoryEx.Create(addons);
            DirectoryEx.Create(cfg);
            DirectoryEx.Create(custom);
        }

        private static void MoveRegistry()
        {
            registryKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Source\\" + game.ShortName + "\\Settings", true);

            if (registryKey == null)
                return;

            regValueNames = registryKey.GetValueNames();
            regValues = new object[regValueNames.Length];

            for (int i = 0; i < regValueNames.Length; i++)
                regValues[i] = registryKey.GetValue(regValueNames[i]);
        }

        private static void CreateAddons()
        {
            if (game.ShortName == "csgo")
                DirectoryEx.Copy("moviefiles\\addons", addons);
        }

        private static void CreateConfig()
        {
            StreamWriter writer = new StreamWriter(cfg + "\\config.cfg");

            writer.WriteLine("sv_cheats 1");
            writer.WriteLine("alias cinematic_off \"mat_bloom_scalefactor_scalar 1;mat_motion_blur_enabled 0;mat_motion_blur_forward_enabled 0;mat_motion_blur_rotation_intensity 1;mat_motion_blur_strength 1\"");
            writer.WriteLine("alias cinematic_normal \"mat_bloom_scalefactor_scalar 2;mat_motion_blur_enabled 1;mat_motion_blur_forward_enabled 1;mat_motion_blur_rotation_intensity 8;mat_motion_blur_strength 2\"");
            writer.WriteLine("alias cinematic_extreme \"mat_bloom_scalefactor_scalar 4;mat_motion_blur_enabled 1;mat_motion_blur_forward_enabled 1;mat_motion_blur_rotation_intensity 8;mat_motion_blur_strength 2\"");

            switch (profile.DefaultCinematicMode)
            {
                case 0:
                default:
                    writer.WriteLine("cinematic_off");
                    break;
                case 1:
                    writer.WriteLine("cinematic_normal");
                    break;
                case 2:
                    writer.WriteLine("cinematic_extreme");
                    break;
            }

            writer.WriteLine();

            if (profile.EnableRecording)
            {
                writer.WriteLine("alias selectfpsuser \"host_timescale {0};host_framerate {1}\"", Math.Round(1.0 / profile.Framerate, 6, MidpointRounding.AwayFromZero).ToString(MainForm.US), profile.Framerate);
                for (int i = 0; i < 10; i++)
                    writer.WriteLine("alias selectfps{0} \"host_timescale {1};host_framerate {2}\"", i, Math.Round(Math.Pow(2, -i) / 30d, 6, MidpointRounding.AwayFromZero).ToString(MainForm.US), 30d * Math.Pow(2, i));
                writer.WriteLine("alias unselectfps \"host_timescale 1;host_framerate 0\"");
                writer.WriteLine();

                writer.WriteLine("alias fpsuser \"echo Frame rate = {0} (user);alias selectfps selectfpsuser;alias fpsup fps0;alias fpsdown fps9\"", profile.Framerate);
                writer.WriteLine("alias fps0 \"echo Frame rate = 30;alias selectfps selectfps0;alias fpsup fps1;alias fpsdown fpsuser\"");
                for (int i = 1; i <= 8; i++)
                    writer.WriteLine("alias fps{0} \"echo Frame rate = {3};alias selectfps selectfps{0};alias fpsup fps{1};alias fpsdown fps{2}\"", i, i + 1, i - 1, 30 * Math.Pow(2, i));
                writer.WriteLine("alias fps9 \"echo Frame rate = 15360;alias selectfps selectfps0;alias fpsup fpsuser;alias fpsdown fps8\"");
                writer.WriteLine("fpsuser");
                writer.WriteLine();

                writer.WriteLine("alias record_start \"{0}gameui_hide;demo_resume;selectfps\"", profile.Config != "" && profile.ConfigExecutionOnRecord ? "exec user;" : "", profile.Framerate, Math.Round((1.0 / profile.Framerate), 6, MidpointRounding.AwayFromZero).ToString(MainForm.US));
                writer.WriteLine("alias record_stop \"endmovie;unselectfps\"");
                writer.WriteLine();

                int last;

                for (last = 1; last <= 27; last++)
                {
                    if (!File.Exists(profile.TgaPath + "\\" + (char)(0x60 + last) + last.ToString() + "_0000.tga") || last == 27)
                    {
                        writer.WriteLine("alias recordtga \"recordtga{0}_start\"", last);
                        break;
                    }
                }

                for (int i = last + 1; i <= 27; i++)
                {
                    if (!File.Exists(profile.TgaPath + "\\" + (char)(0x60 + i) + i.ToString() + "_0000.tga") || i == 27)
                    {
                        if (last == -1)
                        {
                            last = i;
                            continue;
                        }

                        writer.WriteLine("alias recordtga{0}_start \"alias recordtga recordtga{0}_stop;bind F10 recordmoverror;record_start;exec tga{0}.cfg\"", last);
                        writer.WriteLine("alias recordtga{0}_stop \"record_stop;alias recordtga recordtga{1}_start;bind F10 recordmov\"", last, i);
                        File.WriteAllText(cfg + "\\tga" + last.ToString() + ".cfg", "startmovie \"" + Path.GetFullPath(profile.TgaPath) + "\\" + (char)(0x60 + last) + last.ToString() + "_\" raw");

                        last = i;
                    }
                }

                writer.WriteLine("alias recordtga27_start \"play common\\wpn_denyselect\"");
                writer.WriteLine();

                writer.WriteLine("alias recordmov \"recordmov1_start\"");

                for (int i = 1; i <= 26; i++)
                {
                    writer.WriteLine("alias recordmov{0}_start \"alias recordmov recordmov{0}_stop;bind F9 recordtgaerror;record_start;startmovie {1}{0}.mp4 h264\"", i, (char)(i + 0x60));
                    writer.WriteLine("alias recordmov{0}_stop \"record_stop;alias recordmov recordmov{1}_start;bind F9 recordtga\"", i, i + 1);
                }

                writer.WriteLine("alias recordmov27_start \"play common\\wpn_denyselect\"");
                writer.WriteLine();
                writer.WriteLine("alias recordtgaerror \"echo ERROR: Already recording a MP4-Video. Please finish it first before recording a TGA-Sequence.\"");
                writer.WriteLine("alias recordmoverror \"echo ERROR: Already recording a TGA-Sequence. Please finish it first before recording a MP4-Video.\"");
                writer.WriteLine();
                writer.WriteLine("bind F3 \"fpsdown\"");
                writer.WriteLine("bind F4 \"fpsup\"");
                writer.WriteLine("bind F9 \"recordtga\"");
                writer.WriteLine("bind F10 \"recordmov\"");
                writer.WriteLine("bind F11 \"screenshot\"");
                writer.WriteLine();
            }

            if (profile.EnableBinds)
            {
                writer.WriteLine("bind F1 \"toggleconsole\"");
                writer.WriteLine("bind F2 \"hud_reloadscheme{0}\"", profile.Config != "" ? ";exec user.cfg" : "");
                writer.WriteLine("bind F5 \"incrementvar viewmodel_fov_demo 0 180 -5\"");
                writer.WriteLine("bind F6 \"incrementvar viewmodel_fov_demo 0 180 5\"");
                writer.WriteLine("bind F7 \"incrementvar demo_fov_override 0 180 -5\"");
                writer.WriteLine("bind F8 \"incrementvar demo_fov_override 0 180 5\"");
                writer.WriteLine();

                writer.WriteLine("bind Q \"incrementvar  r_drawothermodels 0 2 1\"");
                writer.WriteLine("bind W \"{0}incrementvar mat_wireframe 0 3 1\"", profile.WireframeWorkaround ? "sv_allow_wait_command 1;mat_queue_mode -1;wait 1;mat_queue_mode 0;" : "");
                writer.WriteLine("bind E \"cinematic_off\"");
                writer.WriteLine("bind R \"cinematic_normal\"");
                writer.WriteLine("bind T \"cinematic_extreme\"");
                writer.WriteLine("bind X \"toggle cl_drawhud\"");
                writer.WriteLine("bind C \"toggle crosshair\"");
                writer.WriteLine("bind V \"toggle r_drawviewmodel\"");
                writer.WriteLine("bind B \"toggle r_depthoverlay\"");
                writer.WriteLine();

                if (profile.DemoPlaybackFeatures)
                {
                    for (int i = -20; i <= 20; i++)
                        writer.WriteLine("alias demoscale{0} \"alias demoscaleup demoscale{1};alias demoscaledown demoscale{2};demo_timescale {3}\";",
                            i, i + 1, i - 1, Math.Round(Math.Pow(1.25, i), 6, MidpointRounding.AwayFromZero).ToString(MainForm.US));
                    writer.WriteLine();

                    writer.WriteLine("alias togglethirdperson \"togglethirdperson_on\"");
                    writer.WriteLine("alias togglethirdperson_on \"alias togglethirdperson togglethirdperson_off;thirdperson\"");
                    writer.WriteLine("alias togglethirdperson_off \"alias togglethirdperson togglethirdperson_on;firstperson\"");
                    writer.WriteLine();

                    writer.WriteLine("alias togglemayamode \"togglemayamode_on\"");
                    writer.WriteLine("alias togglemayamode_on \"alias togglemayamode togglemayamode_off;thirdperson_mayamode 1\"");
                    writer.WriteLine("alias togglemayamode_off \"alias togglemayamode togglemayamode_on;thirdperson_mayamode 0\"");
                    writer.WriteLine();

                    writer.WriteLine("bind 0 \"demo_timescale 0\"");
                    writer.WriteLine("bind 1 \"demo_timescale 1\"");
                    writer.WriteLine("bind 2 \"demo_timescale 2\"");
                    writer.WriteLine("bind 3 \"demo_timescale 3\"");
                    writer.WriteLine("bind 4 \"demo_timescale 4\"");
                    writer.WriteLine("bind 5 \"demo_timescale 5\"");
                    writer.WriteLine("bind 6 \"demo_timescale 6\"");
                    writer.WriteLine("bind 7 \"demo_timescale 7\"");
                    writer.WriteLine("bind 8 \"demo_timescale 8\"");
                    writer.WriteLine("bind 9 \"demo_timescale 9\"");
                    writer.WriteLine();

                    writer.WriteLine("bind MOUSE1 \"demo_resume\"");
                    writer.WriteLine("bind MOUSE2 \"demo_pause\"");
                    writer.WriteLine("bind MOUSE3 \"demoscale0\"");
                    writer.WriteLine("bind MWHEELUP \"demoscaleup\"");
                    writer.WriteLine("bind MWHEELDOWN \"demoscaledown\"");
                    writer.WriteLine("bind SPACE \"demo_togglepause\"");
                    writer.WriteLine();
                    writer.WriteLine("demoscale0");
                    writer.WriteLine();
                }

                if (profile.ThirdPersonFeatures)
                {
                    writer.WriteLine("bind HOME \"cam_idealdelta 8");
                    writer.WriteLine("bind END \"cam_idealdelta 1");
                    writer.WriteLine("bind PGUP \"incrementvar cam_idealdelta 1 8 1");
                    writer.WriteLine("bind PGDN \"incrementvar cam_idealdelta 1 8 -1");
                    writer.WriteLine();

                    writer.WriteLine("bind UPARROW \"+campitchup\"");
                    writer.WriteLine("bind DOWNARROW \"+campitchdown\"");
                    writer.WriteLine("bind LEFTARROW \"+camyawleft\"");
                    writer.WriteLine("bind RIGHTARROW \"+camyawright\"");
                    writer.WriteLine();

                    writer.WriteLine("bind KP_UPARROW \"incrementvar cam_idealdistup -512 512 4\"");
                    writer.WriteLine("bind KP_DOWNARROW \"incrementvar cam_idealdistup -512 512 -4\"");
                    writer.WriteLine("bind KP_LEFTARROW \"incrementvar cam_idealdistright -512 512 -4\"");
                    writer.WriteLine("bind KP_RIGHTARROW \"incrementvar cam_idealdistright -512 512 4\"");
                    writer.WriteLine("bind KP_5 \"cam_idealdistup 0;cam_idealdistright 0\"");
                    writer.WriteLine("bind KP_HOME \"incrementvar cam_idealdistup -512 512 4; incrementvar cam_idealdistright -512 512 -4\"");
                    writer.WriteLine("bind KP_PGUP \"incrementvar cam_idealdistup -512 512 4; incrementvar cam_idealdistright -512 512 4\"");
                    writer.WriteLine("bind KP_END \"incrementvar cam_idealdistup -512 512 -4; incrementvar cam_idealdistright -512 512 -4\"");
                    writer.WriteLine("bind KP_PGDN \"incrementvar cam_idealdistup -512 512 -4; incrementvar cam_idealdistright -512 512 4\"");
                    writer.WriteLine("bind KP_PLUS \"+camin\"");
                    writer.WriteLine("bind KP_MINUS \"+camout\"");
                    writer.WriteLine("bind KP_INS \"togglemayamode\"");
                    writer.WriteLine("bind KP_ENTER \"togglethirdperson\"");
                    writer.WriteLine();

                    writer.WriteLine("bind CTRL \"+cammousemove\"");
                    writer.WriteLine("bind ALT \"+camdistance\"");
                    writer.WriteLine();

                    writer.WriteLine("c_maxpitch 360");
                    writer.WriteLine("c_minpitch -360");
                    writer.WriteLine("c_maxyaw 360");
                    writer.WriteLine("c_minyaw -360");
                    writer.WriteLine("c_maxdistance 1600");
                    writer.WriteLine("c_mindistance 0");
                    writer.WriteLine("cam_idealdelta 1");
                    writer.WriteLine();
                }
            }

            if (profile.Config != "")
            {
                File.Copy("moviefiles\\cfg\\" + profile.Config, cfg + "\\user.cfg", true);
                writer.WriteLine("exec user.cfg");
                writer.WriteLine();
            }


            if (demo == null && RecordingRanges.Count > 0)
                demo = RecordingRanges[0].FullPath;

            if (demo != null)
            {
                writer.WriteLine("playdemo \"{0}\"", demo);
                demo = null;
            }

            writer.Close();
        }

        private static void CreateCustom()
        {
            for (int i = 0; i < profile.customItems.Length; i++)
                DirectoryEx.Copy("moviefiles\\custom\\" + profile.customItems[i], custom + "\\" + profile.customItems[i]);
        }

        private static void CreateSkybox()
        {
            if (profile.Skybox == null)
                return;

            string skyboxPath = game.ShortNamePath + "\\custom\\custom_skybox\\materials\\skybox\\";

            Directory.CreateDirectory(skyboxPath);

            for (int i = 0; i < SRTSkybox.Sides.Length; i++)
                File.Copy(profile.Skybox.GetVTF(i), skyboxPath + Path.GetFileName(profile.Skybox.GetVTF(i)), true);

            foreach (string skyName in game.SkyNames)
                for (int i = 0; i < SRTSkybox.Sides.Length; i++)
                    File.Copy(profile.Skybox.GetVMT(i), skyboxPath + skyName + SRTSkybox.Sides[i] + ".vmt", true);
        }

        private static void CreateVDM()
        {
            while (RecordingRanges.Count > 0)
            {
                localRecordingRange.Add(RecordingRanges[0]);
                RecordingRanges.RemoveAt(0);

                for (int i = RecordingRanges.Count - 1; i >= 0; i--)
                {
                    if (RecordingRanges[i].FullPath == localRecordingRange[0].FullPath)
                    {
                        localRecordingRange.Add(RecordingRanges[i]);
                        RecordingRanges.RemoveAt(i);
                    }
                }

                localRecordingRange.Sort((a, b) => a.startTick.CompareTo(b.startTick));

                for (int i = 0; i < localRecordingRange.Count - 1; i++)
                {
                    if (localRecordingRange[i].endTick >= localRecordingRange[i + 1].startTick)
                    {
                        Dialogs.Error(String.Format("Invalid intersection of two ranges:\r\n{0}\r\n{1}", localRecordingRange[i].ToString(), localRecordingRange[i + 1].ToString()));
                        RecordingRanges.Clear();
                        localRecordingRange.Clear();
                        return;
                    }
                }

                int vdmIndex = 1, demoIndex = 1, currentTick = 0;
                vdmFileList.Add(localRecordingRange[0].path + "\\" + Path.GetFileNameWithoutExtension(localRecordingRange[0].name) + ".vdm");
                StreamWriter vdm = new StreamWriter(vdmFileList[vdmFileList.Count - 1]);
                vdm.WriteLine("demoactions");
                vdm.WriteLine("{");

                for (int i = 0; i < localRecordingRange.Count; i++)
                {
                    int startmovie = localRecordingRange[i].startTick;
                    int endmovie = localRecordingRange[i].endTick;
                    int skipAhead = startmovie - 500;

                    if (skipAhead - currentTick >= 500)
                    {
                        vdm.WriteLine("\t\"{0}\"", vdmIndex.ToString());
                        vdm.WriteLine("\t{");
                        vdm.WriteLine("\t\tfactory \"SkipAhead\"");
                        vdm.WriteLine("\t\tname \"{0}\"", vdmIndex.ToString());
                        vdm.WriteLine("\t\tstarttick \"{0}\"", currentTick);
                        vdm.WriteLine("\t\tskiptotick \"{0}\"", skipAhead.ToString());
                        vdm.WriteLine("\t}");
                        vdmIndex++;
                    }

                    vdm.WriteLine("\t\"{0}\"", vdmIndex.ToString());
                    vdm.WriteLine("\t{");
                    vdm.WriteLine("\t\tfactory \"PlayCommands\"");
                    vdm.WriteLine("\t\tname \"{0}\"", vdmIndex);
                    vdm.WriteLine("\t\tstarttick \"{0}\"", startmovie.ToString());

                    switch (profile.ScheduledRecordingMode)
                    {
                        case 0:
                        default:
                            vdm.WriteLine("\t\tcommands \"recordtga\"");
                            break;
                        case 1:
                            vdm.WriteLine("\t\tcommands \"recordmov\"");
                            break;
                    }

                    vdm.WriteLine("\t}");
                    vdmIndex++;

                    vdm.WriteLine("\t\"{0}\"", vdmIndex.ToString());
                    vdm.WriteLine("\t{");
                    vdm.WriteLine("\t\tfactory \"PlayCommands\"");
                    vdm.WriteLine("\t\tname \"{0}\"", vdmIndex);
                    vdm.WriteLine("\t\tstarttick \"{0}\"", endmovie.ToString());

                    switch (profile.ScheduledRecordingMode)
                    {
                        case 0:
                        default:
                            vdm.Write("\t\tcommands \"recordtga");
                            break;
                        case 1:
                            vdm.Write("\t\tcommands \"recordmov");
                            break;
                    }

                    if (i == localRecordingRange.Count - 1)
                    {
                        if (RecordingRanges.Count > 0)
                        {
                            File.WriteAllText(cfg + "\\demo" + demoIndex.ToString() + ".cfg", "playdemo \"" + RecordingRanges[0].FullPath + "\"");
                            vdm.WriteLine(";exec demo{0}\"", demoIndex.ToString());
                            demoIndex++;
                        }
                        else
                            vdm.WriteLine(";quit\"");
                    }
                    else
                    {
                        vdm.WriteLine("\"");
                        vdmIndex++;

                        currentTick = localRecordingRange[i].endTick + 1;
                    }

                    vdm.WriteLine("\t}");
                }

                vdm.WriteLine("}");

                vdm.Close();
                localRecordingRange.Clear();
            }
        }

        private static void Cleanup()
        {
            DirectoryEx.Delete(addons + "_delete");
            DirectoryEx.Delete(cfg + "_delete");
            DirectoryEx.Delete(custom + "_delete");

            foreach (string file in vdmFileList)
                File.Delete(file);

            vdmFileList.Clear();
        }

        private static void MoveH264Videos()
        {
            string source;
            string target;

            for (int i = 1; i <= 26; i++)
            {
                source = game.ShortNamePath + "\\" + (char)(i + 0x60) + i.ToString() + ".mp4";
                target = profile.VideoPath + "\\" + File.GetCreationTime(source).ToString("yyyy-MM-dd HH-mm-ss") + " " + (char)(0x60 + i) + i.ToString() + ".mp4";

                if (File.Exists(source))
                    File.Move(source, target);
                else
                    break;
            }
        }

        private static void MoveRegistryBack()
        {
            if (registryKey == null)
                return;

            for (int i = 0; i < regValueNames.Length; i++)
                registryKey.SetValue(regValueNames[i], regValues[i]);

            registryKey.Dispose();
        }

        private static void MoveFoldersBack()
        {
            Win32.MoveFile(addons, addons + "_delete");
            Win32.MoveFile(cfg, cfg + "_delete");
            Win32.MoveFile(custom, custom + "_delete");

            Win32.MoveFile(addons + "_play", addons);
            Win32.MoveFile(cfg + "_play", cfg);
            Win32.MoveFile(custom + "_play", custom);
        }

        private static void StartGame()
        {
            Process hl2 = new Process();
            hl2.StartInfo = new ProcessStartInfo(game.HL2FileName, String.Format("-game {0} -dxlevel {1} -insecure -novid -console -sw -noborder -w {2} -h {3} -high", game.ShortName, profile.DXLevel, profile.Width, profile.Height));
            hl2.Start();
            hl2.WaitForExit();
            hl2.Dispose();
        }
    }
}
