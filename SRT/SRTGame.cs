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

            Common = steamPath.Replace("/", "\\") + "\\SteamApps\\common";
        }

        public SRTGame(int appID, string name, string longName, string shortName, string executable, params string[] skyNames)
        {
            this.AppID = appID;
            this.Name = name;
            this.LongName = longName;
            this.ShortName = shortName;
            this.Executable = executable;
            this.SkyNames = skyNames;
            this.LongNamePath = Common + "\\" + LongName;
            this.ShortNamePath = LongNamePath + "\\" + ShortName;
            this.HL2FileName = LongNamePath + "\\" + Executable;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
