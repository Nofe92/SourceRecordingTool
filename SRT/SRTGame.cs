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

        static SRTGame()
        {
            string steamPath;

            if ((steamPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", "")) == "")
                throw new Exception("Unable to detect Game Directory.");

            Common = String.Concat(steamPath.Replace("/", "\\"), "\\SteamApps\\common");
        }

        public SRTGame(int appID, string name, string longName, string shortName, string executable, params string[] skyNames)
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
