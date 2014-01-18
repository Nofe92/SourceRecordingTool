using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace SourceRecordingTool
{
    public class SRTGame
    {
        public static SRTGame[] AllGames;
        public static string Common;

        public int AppID;
        public string Name;
        public string LongName;
        public string ShortName;
        public string Executable;
        public string LongNamePath;
        public string ShortNamePath;
        public string HL2FileName;
        public string[] SkyNames;

        internal static void Initialize(MainForm mainForm)
        {
            string steamPath;

            if ((steamPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", "")) == "")
                throw new Exception("Unable to detect Game Directory.");

            Common = String.Concat(steamPath.Replace("/", "\\"), "\\SteamApps\\common");

            AllGames = new SRTGame[] {
                new SRTGame(730, "Counter-Strike: Global Offensive", "Counter-Strike Global Offensive", "csgo", "csgo.exe",
                    "cs_baggage_skybox_", "cs_tibet", "embassy", "italy", "jungle", "office", "sky_cs15_daylight01_hdr", "sky_cs15_daylight02_hdr", "sky_cs15_daylight03_hdr", "sky_cs15_daylight04_hdr", "sky_day02_05", "sky_day02_05_hdr", "sky_dust", "vertigoblue_hdr", "vertigo", "vertigo_hdr", "vietnam"),
                new SRTGame(240, "Counter-Strike: Source", "Counter-Strike Source", "cstrike", "hl2.exe",
                    "assaultup.vmt", "cxup.vmt", "de_cobbleup.vmt", "de_cobble_hdrup.vmt", "de_piranesiup.vmt", "havup.vmt", "italyup.vmt", "jungleup.vmt", "militia_hdrup.vmt", "officeup.vmt", "sky_c17_05up.vmt", "sky_dustup.vmt", "sky_dust_hdrup.vmt", "tidesup.vmt", "trainup.vmt", "train_hdrup.vmt"),
                new SRTGame(300, "Day of Defeat: Source", "Day of Defeat Source", "dod", "hl2.exe",
                    "sky_day01_01", "sky_dod_01_hdr", "sky_dod_02_hdr", "sky_dod_03_hdr", "sky_dod_04_hdr", "sky_dod_05_hdr", "sky_dod_06_hdr", "sky_dod_07z_hdr", "sky_dod_07_hdr", "sky_dod_08_hdr", "sky_dod_09_hdr", "sky_dod_10_hdr"),
                new SRTGame(220, "Half-Life 2", "Half-Life 2", "hl2", "hl2.exe",
                    "sky_borealis01", "sky_day01_01", "sky_day01_01_hdr", "sky_day01_04", "sky_day01_04_hdr", "sky_day01_05", "sky_day01_05_hdr", "sky_day01_06", "sky_day01_06_hdr", "sky_day01_07", "sky_day01_07_hdr", "sky_day01_08", "sky_day01_08_hdr", "sky_day01_09", "sky_day01_09_hdr", "sky_day02_01", "sky_day02_01_hdr", "sky_day02_02", "sky_day02_02_hdr", "sky_day02_03", "sky_day02_03_hdr", "sky_day02_04", "sky_day02_04_hdr", "sky_day02_05", "sky_day02_05_hdr", "sky_day02_06", "sky_day02_06_hdr", "sky_day02_07", "sky_day02_07_hdr", "sky_day02_09", "sky_day02_09_hdr", "sky_day02_10", "sky_day02_10_hdr", "sky_day03_01", "sky_day03_01_hdr", "sky_day03_02", "sky_day03_02_hdr", "sky_day03_03", "sky_day03_04", "sky_day03_04_hdr", "sky_day03_05", "sky_day03_05_hdr", "sky_day03_06b", "sky_day03_06b_hdr", "sky_day03_06", "sky_day03_06_hdr", "sky_wasteland02"),
                new SRTGame(320, "Half-Life 2: Deathmatch", "Half-Life 2 Deathmatch", "hl2mp", "hl2.exe",
                    "sky_borealis01", "sky_day01_01", "sky_day01_01_hdr", "sky_day01_04", "sky_day01_04_hdr", "sky_day01_05", "sky_day01_05_hdr", "sky_day01_06", "sky_day01_06_hdr", "sky_day01_07", "sky_day01_07_hdr", "sky_day01_08", "sky_day01_08_hdr", "sky_day01_09", "sky_day01_09_hdr", "sky_day02_01", "sky_day02_01_hdr", "sky_day02_02", "sky_day02_02_hdr", "sky_day02_03", "sky_day02_03_hdr", "sky_day02_04", "sky_day02_04_hdr", "sky_day02_05", "sky_day02_05_hdr", "sky_day02_06", "sky_day02_06_hdr", "sky_day02_07", "sky_day02_07_hdr", "sky_day02_09", "sky_day02_09_hdr", "sky_day02_10", "sky_day02_10_hdr", "sky_day03_01", "sky_day03_01_hdr", "sky_day03_02", "sky_day03_02_hdr", "sky_day03_03", "sky_day03_04", "sky_day03_04_hdr", "sky_day03_05", "sky_day03_05_hdr", "sky_day03_06b", "sky_day03_06b_hdr", "sky_day03_06", "sky_day03_06_hdr", "sky_wasteland02"),
                new SRTGame(380, "Half-Life 2: Episode One", "Half-Life 2", "episodic", "hl2.exe",
                    "sky_day03_06c", "sky_ep01_00", "sky_ep01_00_hdr", "sky_ep01_01", "sky_ep01_02", "sky_ep01_02_hdr", "sky_ep01_04a", "sky_ep01_04a_hdr", "sky_ep01_04", "sky_ep01_04_hdr", "sky_ep01_citadel_int", "sky_fog"),
                new SRTGame(420, "Half-Life 2: Episode Two", "Half-Life 2", "ep2", "hl2.exe",
                    "sky_ep02_01", "sky_ep02_01_hdr", "sky_ep02_02", "sky_ep02_02_hdr", "sky_ep02_03", "sky_ep02_03_hdr", "sky_ep02_04", "sky_ep02_04_hdr", "sky_ep02_05", "sky_ep02_05_hdr", "sky_ep02_06", "sky_ep02_06_hdr", "sky_ep02_07", "sky_ep02_caves_hdr"),
                new SRTGame(340, "Half-Life 2: Lost Coast", "Half-Life 2", "lostcoast", "hl2.exe",
                    "sky_lostcoast_hdr"),
                new SRTGame(500, "Left 4 Dead", "left 4 dead", "left4dead", "left4dead.exe",
                    "nighturban01_hdr", "sky_day01_09_hdr", "sky_day01_09_ldr", "sky_l4d_rural02_hdr", "sky_l4d_rural02_ldr", "sky_l4d_urban01_hdr", "test_moon_hdr", "urbannightburning_hdr", "urbannightburning_ldr", "urbannightstormhdr", "urbannightstorm_ldr"),
                new SRTGame(550, "Left 4 Dead 2", "Left 4 Dead 2", "left4dead2", "left4dead2.exe",
                    "sky_l4d_c1_1_hdr", "sky_l4d_c1_2_hdr", "sky_l4d_c2m1_hdr", "sky_l4d_c4m1_hdr", "sky_l4d_c4m4_hdr", "sky_l4d_c5_1_hdr", "sky_l4d_night02_hdr", "sky_l4d_predawn02_hdr", "sky_l4d_c6m1_hdr", "docks_hdr", "highrise_hdr", "river_hdr", "sky_l4d_urban01_hdr", "test_moon_hdr", "urbannightstormhdr", "urbannightstorm_ldr", "sky_coldstream_1_hdr", "sky_coldstream_2_hdr", "sky_day01_09_hdr", "sky_day01_09_ldr", "sky_l4d_rural02_hdr", "sky_l4d_rural02_ldr", "urbannightburning_hdr", "urbannightburning_ldr"),
                new SRTGame(400, "Portal", "Portal", "portal", "hl2.exe",
                    "bts_custom_cubemap", "sky_escape_01_"),
                new SRTGame(620, "Portal 2", "Portal 2", "portal2", "portal2.exe",
                    "sky_black", "sky_black_nofog", "sky_fog", "sky_l4d_c4m1_hdr", "sky_white"),
                new SRTGame(440, "Team Fortress 2", "Team Fortress 2", "tf", "hl2.exe",
                    "sky_alpinestorm_01", "sky_badlands_01", "sky_dustbowl_01", "sky_dustbowl_01_hdr", "sky_goldrush_01", "sky_granary_01", "sky_granary_01_hdr", "sky_gravel_01", "sky_gravel_01_hdr", "sky_halloween", "sky_halloween_night_01", "sky_halloween_night_01_hdr", "sky_harvest_01", "sky_harvest_01_hdr", "sky_harvest_night_01", "sky_hydro_01", "sky_hydro_01_hdr", "sky_morningsnow_01", "sky_nightfall_01", "sky_night_01", "sky_rainbow_01", "sky_stormfront_01", "sky_tf2_04", "sky_tf2_04_hdr", "sky_trainyard_01", "sky_upward", "sky_upward_hdr", "sky_well_01", "sky_well_01_hdr"),
            };

            mainForm.GameComboBox.DataSource = AllGames;
        }

        private SRTGame(int appID, string name, string longName, string shortName, string executable, params string[] skyNames)
        {
            this.AppID = appID;
            this.Name = name;
            this.LongName = longName;
            this.ShortName = shortName;
            this.Executable = executable;
            this.SkyNames = skyNames;
            this.LongNamePath = String.Concat(Common, "\\", LongName);
            this.ShortNamePath = String.Concat(LongNamePath, "\\", ShortName);
            this.HL2FileName = String.Concat(LongNamePath, "\\", Executable);
        }

        public void EnsureRequirements()
        {
            Process[] hl2 = Process.GetProcessesByName(Executable.Substring(0, Executable.Length - 4));

            foreach (Process process in hl2)
                process.Dispose();

            if (hl2.Length != 0)
                throw new Exception("Only one instance of the game can be running at one time.");

            if (!File.Exists(HL2FileName))
                throw new Exception("Unable to find executable.");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
